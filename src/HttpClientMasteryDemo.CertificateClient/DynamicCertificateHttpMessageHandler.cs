namespace HttpClientMasteryDemo.CertificateClient;

internal sealed class DynamicCertificateHttpMessageHandler : DelegatingHandler
{
    private readonly ICertificateProvider _certificateProvider;
    private static readonly HttpClientHandler _handler = new();

    public DynamicCertificateHttpMessageHandler(ICertificateProvider certificateProvider)
    {
        _certificateProvider = certificateProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Console.WriteLine("====================================================");
        Console.WriteLine($"DynamicCertificateHttpMessageHandler.SendAsync @ {DateTime.Now:G}");
        
        var clientCertificate = await _certificateProvider.GetLatestCertificate();
        if (clientCertificate != null)
        {
            _handler.ClientCertificates.Add(clientCertificate);
        }

        using var innerHttpClient = new HttpClient(_handler, disposeHandler: false)
        {
            BaseAddress = request.RequestUri
        };

        var clonedRequest = await CloneHttpRequestMessageAsync(request);
        return await innerHttpClient.SendAsync(clonedRequest, cancellationToken);
    }

    private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version
        };

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (request.Content != null)
        {
            var ms = new System.IO.MemoryStream();
            await request.Content.CopyToAsync(ms);
            ms.Position = 0;
            clone.Content = new StreamContent(ms);
            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }
}
