namespace BoxwriterResmarkInterop.OPCUA;

using System.Text.RegularExpressions;

using Exceptions;

using Interfaces;

using Mediator;

using TCP;

public class GetTaskCommandHandler : IRequestHandler<GetTaskRequest, StringResponse>
{
    private readonly ILogger<GetTaskCommandHandler> _logger;
    private readonly IOPCUAService _opcuaService;
    private const string GetTaskRegex = @",\s*(.+)\s*}";


    public GetTaskCommandHandler(ILogger<GetTaskCommandHandler> logger, IOPCUAService opcuaService)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    public async ValueTask<StringResponse> Handle(GetTaskRequest request, CancellationToken cancellationToken)
    {
        var printerIdRegex = ExtractPrinterId(request.data);

        var printerId = printerIdRegex.Groups[1].Value;

        var response = await _opcuaService.CallMethodAsync(printerId, "GetStoredMessageList", cancellationToken).ConfigureAwait(false);

        if (response is null)
        {
            _logger.LogError("OPCUA call failed");

            throw new OPCUACommunicationFailedException("OPCUA call failed");
        }

        var outputArguments = response.First()?.OutputArguments ?? throw new IndexOutOfRangeException();

        if (!(outputArguments.Length > 1))
        {
            _logger.LogError("Output argument is not correct length");

            throw new InvalidDataException("Output argument is not correct length");
        }

        return new StringResponse("{Get tasks, printerId, " + string.Join(',', outputArguments[1].Value as string[] ?? new[] { "No messages" }) + "}");
    }

    private Match ExtractPrinterId(string data)
    {
        var printerIdRegex = new Regex(GetTaskRegex).Match(data);

        if (!printerIdRegex.Success)
        {
            _logger.LogError("Unable to find printer ID. Request was {Data}", data);

            throw new InvalidDataException($"Unable to find printer ID. Request was {data}.");
        }

        return printerIdRegex;
    }
}