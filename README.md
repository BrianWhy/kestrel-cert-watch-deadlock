# Kestrel certificate file watch deadlock

A minimal repro for a deadlock in Kestrel's certificate file watching (`CertificatePathWatcher`).
It reproduces on ASP.NET Core 10.0.13 / .NET 10.0.13 (and 10.0.12) on macOS arm64.

The project is the ASP.NET Core Empty template (`dotnet new web`) with only `Program.cs` changed. `Program.cs`
writes a self-signed certificate to a temporary directory once and never changes it. It then starts and stops
Kestrel in a loop, with that certificate configured by path (`Kestrel:Certificates:Default:Path`).

## Run

```
dotnet run -c Release
```

If one start and stop takes longer than 10 seconds, the program prints:

```
Start <n> hung. Stacks: dotnet-stack report -p <pid>
```

Run that command in another terminal to see the two deadlocked threads (`dotnet tool install -g dotnet-stack`),
then press Enter to exit. On a 14-core Mac it hung within 2 to 10 minutes. It is a race, so a run can take longer.
