using System.Net;
using System.Net.Sockets;
using System.Text;

using BoxwriterResmarkInterop.Abstracts;
using BoxwriterResmarkInterop.Extensions;
using BoxwriterResmarkInterop.Requests;

using MediatR;

namespace BoxwriterResmarkInterop.TCP;
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

    public static async Task ProcessDataAsync(string data, NetworkStream stream, CancellationToken cancellationToken = default)
    {
        var response = Encoding.ASCII.GetBytes(data);

        await stream.WriteAsync(response, cancellationToken);

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
                try
                {
                    _logger.LogTrace("Waiting for connection on {IPAddress}", address);

                    using var tcpClient = await server
                        .AcceptTcpClientAsync(stoppingToken);

                    _logger.LogTrace("New connection made to {IPAddress} from {ClientAddress}", address, tcpClient.Client.RemoteEndPoint);

                    var stream = tcpClient.GetStream();
                    var builder = new StringBuilder();
                    var buffer = new byte[256];

                    do
                    {
                        var length = await stream.ReadAsync(buffer, stoppingToken);

                        builder.Append(Encoding.ASCII.GetString(buffer, 0, length));

                    } while (stream.DataAvailable);

                    var data = builder.ToString();

                    _logger.LogInformation("Read data {data} to {IPAddress} from {RemoteAddress}", data, address,
                        tcpClient.Client.RemoteEndPoint);

                    var request = CreateRequest(data);

                    var response = await _mediator.Send(request, stoppingToken);

                    await ProcessDataAsync(response.Data, stream, stoppingToken);
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
            Constants.GetTasks => new GetTasksRequest(data),
            Constants.StartTask => new StartTaskRequest(data),
            Constants.ResumeTask => new ResumeTaskRequest(data),
            Constants.LoadTask => new LoadTaskRequest(data),
            Constants.IdleTask => new IdleTaskRequest(data),
            Constants.GetUserElements => new GetUserElementsRequest(data),
            Constants.SetUserElements => new SetUserElementsRequest(data),
            Constants.AddLine => new AddLineRequest(data),
            Constants.GetLines => new GetLinesRequest(data),
            Constants.SetCount => new SetCountRequest(data),
            _ => throw new InvalidDataException("Data response was malformed.")
        };
    }
}