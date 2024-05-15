using HttpClientMasteryDemo.CertificateAuthority;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<CertificateAuthorityService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<CertificateAuthorityService>());

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/latest", async context =>
{
    var certificateProvider = context.RequestServices.GetRequiredService<CertificateAuthorityService>();
    var certificate = certificateProvider.GetLatestCertificate();
    var bytes = certificate.Export(X509ContentType.Pfx);
    var certData = Convert.ToBase64String(bytes);
    await context.Response.WriteAsync(certData);
});

app.Run();