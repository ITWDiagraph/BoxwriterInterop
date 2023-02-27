namespace BoxwriterResmarkInterop.TCP;

using System.Net;
using System.Net.Sockets;
using System.Text;

using Abstracts;

using Interfaces;

public class BoxwriterTCPWorker : BoxwriterWorkerBase
{
    private const int Port = 2202;
    private readonly ITCPDataHandler _handler;
    private readonly ILogger<BoxwriterTCPWorker> _logger;

    public BoxwriterTCPWorker(ILogger<BoxwriterTCPWorker> logger, ITCPDataHandler handler)
    {
        _logger = logger;
        _handler = handler;
    }

    protected override async Task StartListeningAsync(IPAddress address, CancellationToken stoppingToken)
    {
        TcpListener? server = null;

        try
        {
            server = new TcpListener(address, Port);

            server.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogTrace("Waiting for connection on {IPAddress}", address);

                using var client = await server.AcceptTcpClientAsync(stoppingToken);

                _logger.LogTrace("New connection made on {IPAddress} to {ClientAddress}", address,
                    client.Client.RemoteEndPoint);

                var stream = client.GetStream();
                var builder = new StringBuilder();
                var buffer = new byte[256];
                do
                {
                    var length = await stream.ReadAsync(buffer, 0, buffer.Length, stoppingToken);
                    builder.Append(Encoding.ASCII.GetString(buffer, 0, length));
                } while (stream.DataAvailable);

                var data = builder.ToString();
                _logger.LogInformation("Read data {data} on {IPAddress} from {RemoteAddress}", data, address,
                    client.Client.RemoteEndPoint);

                await _handler.ProcessData(data, stream, stoppingToken);
            }
        }
        catch (SocketException ex)
        {
            _logger.LogCritical("Socket error occured on {IPAddress} with the following error {Error}", address, ex);
        }
        finally
        {
            server?.Stop();
        }
    }
}