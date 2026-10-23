// Starts and stops Kestrel in a loop with a certificate configured by path. The file is written once and never changed.
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

var pfx = Path.Combine(Directory.CreateTempSubdirectory().FullName, "cert.pfx");
File.WriteAllBytes(pfx, new CertificateRequest("CN=localhost", RSA.Create(2048), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)
    .CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1)).Export(X509ContentType.Pfx));

for (int n = 1; ; n++)
{
    var builder = WebApplication.CreateBuilder(["--urls=https://127.0.0.1:0", $"--Kestrel:Certificates:Default:Path={pfx}"]);
    builder.Logging.ClearProviders();
    var app = builder.Build();
    if (!Task.Run(async () => { await app.StartAsync(); await app.StopAsync(); }).Wait(TimeSpan.FromSeconds(10)))
    {
        Console.WriteLine($"Start {n} hung. Stacks: dotnet-stack report -p {Environment.ProcessId}");
        Console.ReadLine();
        return;
    }
    await app.DisposeAsync();
}
