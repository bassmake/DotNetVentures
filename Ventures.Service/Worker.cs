using System.Net.Sockets;
using System.Text;

namespace Ventures.Service;

public class Worker : BackgroundService
{
    public const int ListenPort = 11000;
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var listener = new UdpClient(ListenPort);
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Waiting for broadcast on port: {Port}", ListenPort);

                var result = await listener.ReceiveAsync(stoppingToken);
                var bytes = result.Buffer;
                var data = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                _logger.LogInformation("Received broadcast from {EndPoint}: {Data}", result.RemoteEndPoint, data);
            }
        }
        catch (SocketException e)
        {
            _logger.LogError(e, "Problem with socket");
        }
    }
}