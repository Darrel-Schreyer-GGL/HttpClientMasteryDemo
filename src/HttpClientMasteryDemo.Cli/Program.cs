using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services
                    .AddHttpClient("CertificateAuthority", configureClient =>
                    {
                        configureClient.DefaultRequestHeaders.Add("Accept", "application/json");
                    });

                services
                    .AddHttpClient("ClientWithDynamicCert")
                    .ConfigurePrimaryHttpMessageHandler(sp =>
                    {
                        var handler = new HttpClientHandler();
                        var certificateProvider = sp.GetRequiredService<ICertificateProvider>();
                        var clientCertificate = certificateProvider.GetLatestCertificate();
                        handler.ClientCertificates.Add(clientCertificate);

                        handler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
                        {
                            // Implement custom certificate validation logic
                            return true;
                        };

                        return handler;
                    });
            })
            .Build();

Console.Clear();

var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();

while(true)
{
    Console.WriteLine("Press any key to make a request to the server...");
    Console.ReadKey();

    var client = httpClientFactory.CreateClient("ClientWithDynamicCert");
    var response = await client.GetAsync("https://localhost:5001/");

    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response: {content}");
    }
    else
    {
        Console.WriteLine($"Failed to make a request. Status code: {response.StatusCode}");
    }

    await Task.Delay(30_000);
}
