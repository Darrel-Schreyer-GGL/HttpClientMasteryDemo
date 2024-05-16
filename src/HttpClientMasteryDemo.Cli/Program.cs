using HttpClientMasteryDemo.CertificateClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

await Task.Delay(5000); // Wait for the server to start

var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddCertificateClients();
            })
            .Build();

Console.Clear();

await host.StartAsync();

var httpClientFactory = host.Services.GetRequiredService<IHttpClientFactory>();

try
{
    for (int i = 0; i < 15; i++)
    {
        var dynamicCertClient = httpClientFactory.CreateDynamicCertClient();
        var response = await dynamicCertClient.GetAsync("https://localhost:7001/");
        Console.WriteLine($"Request {i + 1} status: {response.StatusCode}");

        await Task.Delay(500);
    }
}
catch (HttpRequestException httpEx)
{
    Console.WriteLine($"HTTP request error: {httpEx.Message}");
}
catch (InvalidOperationException invalidOpEx)
{
    Console.WriteLine($"Invalid operation: {invalidOpEx.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}

Console.WriteLine("Finished processing requests.");
await host.StopAsync();