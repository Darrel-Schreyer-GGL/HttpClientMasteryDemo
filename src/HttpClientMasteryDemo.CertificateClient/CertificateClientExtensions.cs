using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HttpClientMasteryDemo.CertificateClient;

public static class CertificateClientExtensions
{
    private const string DynamicCertClient = "DynamicCertClient";

    public static void AddCertificateClients(this IServiceCollection services)
    {
        services.TryAddSingleton<ICertificateProvider, CertificateProvider>();
        services.TryAddTransient<DynamicCertificateHttpMessageHandler>();

        services.AddHttpClient(DynamicCertClient)
            .ConfigurePrimaryHttpMessageHandler<DynamicCertificateHttpMessageHandler>();
    }

    public static HttpClient CreateDynamicCertClient(this IHttpClientFactory httpClientFactory)
    {
        return httpClientFactory.CreateClient(DynamicCertClient);
    }
}
