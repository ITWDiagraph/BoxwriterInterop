namespace BoxwriterResmarkInterop.UDP;

using System.Net;
using System.Net.Sockets;

using Abstracts;

using Interfaces;

public class BoxwriterUDPWorker : BoxwriterWorkerBase
{
    private const int Port = 2200;
    private readonly IUdpDataHandler _handler;
    private readonly ILogger<BoxwriterUDPWorker> _logger;

    public BoxwriterUDPWorker(ILogger<BoxwriterUDPWorker> logger, IUdpDataHandler handler)
    {
        _logger = logger;
        _handler = handler;
    }

    protected override async Task ListenAsync(IPAddress address, CancellationToken stoppingToken)
    {
        using var listener = new UdpClient(new IPEndPoint(address, Port));

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.Log(LogLevel.Debug, "Waiting for UDP packet");

            var data = await listener.ReceiveAsync(stoppingToken);

            _logger.Log(LogLevel.Information, "Received UDP packet from {Endpoint}", data.RemoteEndPoint);

            await _handler.ProcessDataAsync(data, address);
        }
    }
}