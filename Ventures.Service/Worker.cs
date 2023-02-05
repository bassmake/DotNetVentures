using System.Net;
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
        var groupEP = new IPEndPoint(IPAddress.Any, ListenPort);
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Waiting for broadcast on {GroupEP}", groupEP);
                
                // var bytes = listener.Receive(ref groupEP);

                // _logger.LogInformation("Received broadcast from {GroupEP} :", groupEP);
                // _logger.LogInformation(" {Received}", Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                
                var result = await listener.ReceiveAsync(stoppingToken);
                _logger.LogInformation("Received broadcast from {IpEndPoint} :", result.RemoteEndPoint);
                
                var bytes = result.Buffer;
                _logger.LogInformation(" {Info}", Encoding.ASCII.GetString(bytes, 0, bytes.Length));
            }
        }
        catch (SocketException e)
        {
            _logger.LogError(e, "Problem with socket");
        }
    }
}