using HttpClientMasteryDemo.CertificateClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCertificateClients();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Cool!");
});

app.Run();