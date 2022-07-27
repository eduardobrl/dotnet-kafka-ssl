using ProcessadorPedidos.Worker;
using ProcessadorPedidos.Worker.Extensions;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.RegisterServices();
    })
    .UseSerilog()
    .Build();

await host.RunAsync();
