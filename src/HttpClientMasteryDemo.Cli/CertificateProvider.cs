using System.Security.Cryptography.X509Certificates;

public sealed class CertificateProvider : ICertificateProvider
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CertificateProvider(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public X509Certificate2 GetLatestCertificate()
    {
        var httpClient = _httpClientFactory.CreateClient("CertificateAuthority");

        var certificateData = httpClient
            .GetStringAsync("https://localhost:7002/latest")
            .GetAwaiter()
            .GetResult();
        var certificateBytes = Convert.FromBase64String(certificateData);

        return new X509Certificate2(certificateBytes);
    }
}