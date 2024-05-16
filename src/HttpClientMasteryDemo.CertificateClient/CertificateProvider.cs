using System.Security.Cryptography.X509Certificates;

namespace HttpClientMasteryDemo.CertificateClient;

public sealed class CertificateProvider : ICertificateProvider
{
    private readonly object _syncLock = new();
    private X509Certificate2? _certificateCache = null;

    public async Task<X509Certificate2?> GetLatestCertificate()
    {
        if (IsCertificateValid)
        {
            return _certificateCache!;
        }

        lock (_syncLock)
        {
            if (!IsCertificateValid)
            {
                //_certificateCache = TODO: Get your certificate here.
            }
        }

        return await Task.FromResult(_certificateCache!);
    }

    private bool IsCertificateValid
    {
        get
        {
            lock (_syncLock)
            {
                return _certificateCache != null && _certificateCache.Verify();
            }
        }
    }

}
