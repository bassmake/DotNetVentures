using System.Net;
using System.Net.Sockets;
using System.Text;
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
    public async Task Sending_simple_UDP()
    {
        var provider = new ServiceCollection()
            .AddLogging(lb => lb.AddSerilog())
            .AddHostedService<Worker>()
            .BuildServiceProvider();
        
        var cts = new CancellationTokenSource();
        
        var worker = provider.GetService<IHostedService>() as Worker;
        var task = worker?.StartAsync(cts.Token)!;

        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        // var broadcast = IPAddress.Parse("192.168.1.255");
        // var broadcast = IPAddress.Parse("0.0.0.0");
        
        // new Socket()
        var client = new UdpClient(Worker.ListenPort + 1);
        var sendBuf = Encoding.ASCII.GetBytes("Hell no!");

        var groupEP = new IPEndPoint(IPAddress.Any, Worker.ListenPort);
        await client.SendAsync(sendBuf, groupEP, cts.Token);
        // await socket.SendAsync(sendBuf, SocketFlags.Broadcast, cts.Token);
        
        await Task.Delay(TimeSpan.FromSeconds(1), CancellationToken.None);
        
        cts.Cancel();
        task.Status.ShouldBe(TaskStatus.RanToCompletion);
    }
}