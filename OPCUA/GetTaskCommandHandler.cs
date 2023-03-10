namespace BoxwriterResmarkInterop.OPCUA;

using System.Text;
using System.Text.RegularExpressions;

using Exceptions;

using Interfaces;

using Mediator;

using Requests;

using Workstation.ServiceModel.Ua;

public class GetTaskCommandHandler : IRequestHandler<GetTaskRequest, StringResponse>
{
    private const string GetTaskRegex = @",\s*(.+)\s*}";
    private readonly ILogger<GetTaskCommandHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public GetTaskCommandHandler(ILogger<GetTaskCommandHandler> logger, IOPCUAService opcuaService)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    public async ValueTask<StringResponse> Handle(GetTaskRequest request, CancellationToken cancellationToken)
    {
        var printerIdRegex = ExtractPrinterId(request.data);

        var printerId = printerIdRegex.Groups[1].Value;

        var response = await _opcuaService.CallMethodAsync(printerId, "GetStoredMessageList", cancellationToken);

        if (response is null)
        {
            _logger.LogError("OPCUA call failed");

            throw new OPCUACommunicationFailedException("OPCUA call failed");
        }

        var outputArguments = response.FirstOrDefault()?.OutputArguments ?? throw new IndexOutOfRangeException();

        if (!(outputArguments.Length > 1))
        {
            _logger.LogError("Output argument is not correct length");

            throw new InvalidDataException("Output argument is not correct length");
        }

        return FormatResponse(outputArguments, printerId);
    }

    private StringResponse FormatResponse(Variant[] outputArguments, string printerId)
    {
        var responseBuilder = new StringBuilder();

        responseBuilder.Append($@"{{Get tasks, {printerId}, ");

        if (outputArguments[1].Value is string[])
        {
            foreach (var arg in outputArguments[1].Value as string[])
            {
                responseBuilder.Append(arg);
            }
        }
        else
        {
            responseBuilder.Append("No messages");
        }

        responseBuilder.Append("}");

        return new StringResponse(responseBuilder.ToString());
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