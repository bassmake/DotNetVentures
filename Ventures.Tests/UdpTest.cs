using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Shouldly;
using Ventures.Service;

namespace Ventures.Tests;

public class UdpTest
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
    public async Task Sending_simple_datagram()
    {
        var provider = new ServiceCollection()
            .AddLogging(lb => lb.AddSerilog())
            .AddHostedService<Worker>()
            .BuildServiceProvider();
        
        const string message = "Hell NO!";
        
        var cts = new CancellationTokenSource();
        
        var worker = provider.GetService<IHostedService>() as Worker;
        var task = worker?.StartAsync(cts.Token)!;

        var client = new UdpClient(Worker.ListenPort + 1);
        await client.SendAsync(message.ToBytes(), new IPEndPoint(IPAddress.Any, Worker.ListenPort), CancellationToken.None);

        var received = await client.ReceiveAsync(CancellationToken.None);
        
        received.Buffer.ToAscii().ShouldBe(message.Reversed());
        
        await Task.Delay(TimeSpan.FromSeconds(1), CancellationToken.None);
        cts.Cancel();
        
        task.Status.ShouldBe(TaskStatus.RanToCompletion);
    }
}