namespace BoxwriterResmarkInterop.OPCUA;

using System.Text.RegularExpressions;

using Interfaces;

using Mediator;

using TCP;

public class OPCUAGetTaskHandler : IRequestHandler<GetTaskRequest, StringResponse>
{
    private readonly ILogger<OPCUAGetTaskHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public OPCUAGetTaskHandler(ILogger<OPCUAGetTaskHandler> logger, IOPCUAService opcuaService)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    public async ValueTask<StringResponse> Handle(GetTaskRequest request, CancellationToken cancellationToken)
    {
        //Extract printer id
        var data = request.data;

        var printerIdRegex = new Regex(@",\s*(.+)\s*}").Match(data);

        if (!printerIdRegex.Success)
        {
            _logger.LogError("Unable to find printer ID. Request was {data}", data);

            throw new InvalidDataException($"Unable to find printer ID. Request was {data}.");
        }

        var printerId = printerIdRegex.Groups[1].Value;

        //Get stored message list from the printer
        var response = await _opcuaService.CallMethodAsync(printerId, "GetStoredMessageList", cancellationToken);

        //Return formatted response
        var outputArguments = response?[0]?.OutputArguments;

        if (!(outputArguments?.Length > 1))
        {
            _logger.LogError("Output argument is not correct length");

            throw new InvalidDataException();
        }

        return new StringResponse("{Get tasks, printerId, " + string.Join(',', outputArguments[1].Value as string[] ?? new[] { "No messages" }) + "}");
    }
}