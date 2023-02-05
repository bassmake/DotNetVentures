using Serilog;
using Serilog.Events;
using Ventures.Service;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((_, config) => config.MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console())
    .ConfigureServices(services => { services.AddHostedService<Worker>(); })
    .Build();

await host.RunAsync();