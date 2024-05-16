using System.Security.Cryptography.X509Certificates;

namespace HttpClientMasteryDemo.CertificateClient;

public interface ICertificateProvider
{
    Task<X509Certificate2?> GetLatestCertificate();
}
