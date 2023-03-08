namespace BoxwriterResmarkInterop.TCP;

using System.Net;
using System.Net.Sockets;
using System.Text;

using Abstracts;

using Interfaces;

using Mediator;

public class BoxwriterTCPWorker : BoxwriterWorkerBase
{
    private const int Port = 2202;
    private readonly ILogger<BoxwriterTCPWorker> _logger;
    private readonly IMediator _mediator;

    public BoxwriterTCPWorker(ILogger<BoxwriterTCPWorker> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task ProcessDataAsync(string data, NetworkStream stream, CancellationToken cancellationToken = default)
    {
        var response = Encoding.ASCII.GetBytes(data);

        await stream.WriteAsync(response, 0, response.Length, cancellationToken);

        await stream.FlushAsync(cancellationToken);
    }

    protected override async Task ListenAsync(IPAddress address, CancellationToken stoppingToken)
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

                _logger.LogTrace("New connection made to {IPAddress} from {ClientAddress}", address,
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

                _logger.LogInformation("Read data {data} to {IPAddress} from {RemoteAddress}", data, address,
                    client.Client.RemoteEndPoint);

               var response = await _mediator.Send(new TCPRequest(data), stoppingToken);

               await ProcessDataAsync(response.data, stream, stoppingToken);
            }
        }
        catch (SocketException ex)
        {
            _logger.LogCritical("Socket error occurred on {IPAddress}: {Error}", address, ex);
        }
        finally
        {
            server?.Stop();
        }
    }
}