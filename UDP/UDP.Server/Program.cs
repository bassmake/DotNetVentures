using Serilog;
using Serilog.Events;
using UDP.Server;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => { services.AddHostedService<Worker>(); })
    .UseSerilog((_, config) => config.MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console())
    .Build();

await host.RunAsync();