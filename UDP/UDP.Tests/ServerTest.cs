using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using UDP.Server;

namespace UDP.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.NUnitOutput()
            .MinimumLevel.Debug()
            .CreateLogger();
    }

    [Test]
    public async Task Test1()
    {
        var provider = new ServiceCollection()
            .AddLogging(lb => lb.AddSerilog())
            .AddHostedService<Worker>()
            .BuildServiceProvider();
        
        var cts = new CancellationTokenSource();
        
        var worker = provider.GetService<IHostedService>() as Worker;
        var task = worker?.StartAsync(cts.Token)!;

        // logger.LogInformation("Worker started");
        
        await Task.Delay(TimeSpan.FromSeconds(5), CancellationToken.None);
        
        cts.Cancel();
        
        Assert.Pass();
    }
}