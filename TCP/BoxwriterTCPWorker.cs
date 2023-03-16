namespace BoxwriterResmarkInterop.TCP;

using System.Net;
using System.Net.Sockets;
using System.Text;

using Abstracts;

using Extensions;

using MediatR;

using Requests;

using static Constants;

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

        await stream.WriteAsync(response, 0, response.Length, cancellationToken).ConfigureAwait(false);

        await stream.FlushAsync(cancellationToken).ConfigureAwait(false);
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
                try
                {
                    _logger.LogTrace("Waiting for connection on {IPAddress}", address);

                    using var tcpClient = await server
                        .AcceptTcpClientAsync(stoppingToken)
                        .ConfigureAwait(false);

                    _logger.LogTrace("New connection made to {IPAddress} from {ClientAddress}", address, tcpClient.Client.RemoteEndPoint);

                    var stream = tcpClient.GetStream();
                    var builder = new StringBuilder();
                    var buffer = new byte[256];

                    do
                    {
                        var length = await stream
                            .ReadAsync(buffer, 0, buffer.Length, stoppingToken)
                            .ConfigureAwait(false);

                        builder.Append(Encoding.ASCII.GetString(buffer, 0, length));

                    } while (stream.DataAvailable);

                    var data = builder.ToString();

                    _logger.LogInformation("Read data {data} to {IPAddress} from {RemoteAddress}", data, address,
                        tcpClient.Client.RemoteEndPoint);

                    var request = CreateRequest(data);

                    var response = await _mediator.Send(request, stoppingToken)
                        .ConfigureAwait(false);

                    await ProcessDataAsync(response.Data, stream, stoppingToken)
                        .ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.LogError("An exception occurred when receiving data {Exception}", e);
                }
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

    private static IRequest<StringResponse> CreateRequest(string data)
    {
        var commandName = data.ExtractCommandName();

        return commandName switch
        {
            GetTasks => new GetTasksRequest(data),
            StartTask => new StartTaskRequest(data),
            ResumeTask => new ResumeTaskRequest(data),
            LoadTask => new LoadTaskRequest(data),
            IdleTask => new IdleTaskRequest(data),
            AddLine => new AddLineRequest(data),
            GetUserElements => new GetUserElementsRequest(data),
            SetUserElements => new SetUserElementsRequest(data),
            _ => throw new InvalidDataException("Data response was malformed.")
        };
    }
}