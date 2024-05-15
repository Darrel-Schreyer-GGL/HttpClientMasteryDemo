using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace HttpClientMasteryDemo.CertificateAuthority;

public sealed class CertificateAuthorityService : IHostedService, IDisposable
{
    private readonly int _minutesToNewCertificate = 3;
    private Timer _timer = default!;
    private X509Certificate2 _latestCertificate = default!;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _latestCertificate = GenerateSelfSignedCertificate();
        _timer = new Timer(GenerateCertificate, null, TimeSpan.Zero, TimeSpan.FromMinutes(_minutesToNewCertificate));
        return Task.CompletedTask;
    }

    private void GenerateCertificate(object? state)
    {
        _latestCertificate = GenerateSelfSignedCertificate();
    }

    private X509Certificate2 GenerateSelfSignedCertificate()
    {
        using var key = ECDsa.Create();
        var req = new CertificateRequest("CN=TestCert", key, HashAlgorithmName.SHA256);
        var cert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddMinutes(_minutesToNewCertificate));

        // Export and re-import the certificate to ensure the private key is properly associated
        var certBytes = cert.Export(X509ContentType.Pfx);
        return new X509Certificate2(certBytes, (string?)null, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
    }

    public X509Certificate2 GetLatestCertificate() => _latestCertificate;

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
