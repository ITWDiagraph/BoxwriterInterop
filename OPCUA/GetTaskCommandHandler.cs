namespace BoxwriterResmarkInterop.OPCUA;

using System.Text;
using System.Text.RegularExpressions;

using Exceptions;

using Interfaces;

using MediatR;

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

    public async Task<StringResponse> Handle(GetTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = ExtractPrinterId(request.data);

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

    private StringResponse FormatResponse(Variant[]? outputArguments, string? printerId)
    {
        var responseBuilder = new StringBuilder();

        responseBuilder.Append($@"{{Get tasks, {printerId}, ");

        if (outputArguments != null)
        {
            if (outputArguments[1].Value is string[] args)
            {
                foreach (var arg in args)
                {
                    responseBuilder.Append(arg);
                }
            }
            else
            {
                responseBuilder.Append("No messages");
            }
        }

        responseBuilder.Append("}");

        return new StringResponse(responseBuilder.ToString());
    }

    private string ExtractPrinterId(string data)
    {
        var printerIdRegex = new Regex(GetTaskRegex).Match(data);

        if (!printerIdRegex.Success)
        {
            _logger.LogError("Unable to find printer ID. Request was {Data}", data);

            throw new InvalidDataException($"Unable to find printer ID. Request was {data}.");
        }

        return printerIdRegex.Groups[1].Value;
    }
}