namespace BoxwriterResmarkInterop.OPCUA;

using Exceptions;

using Extensions;

using Interfaces;

using MediatR;

using Requests;

using Workstation.ServiceModel.Ua;

using static Constants;

public class GetTaskCommandHandler : ICommandHandler, IRequestHandler<GetTasksRequest, StringResponse>
{
    private const int ExpectedOutputArgsLength = 2;
    private const string NoMessages = "NoMessages";
    private readonly ILogger<GetTaskCommandHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public GetTaskCommandHandler(ILogger<GetTaskCommandHandler> logger, IOPCUAService opcuaService)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    public string CommandName => "Get tasks";

    public string GetResponseData(CallMethodResult result)
    {
        var outputArguments = result.OutputArguments;

        if (outputArguments?[1].Value is string[] args)
        {
            return args.Aggregate((current, arg) => current + TokenSeparator + arg);
        }

        return NoMessages;
    }

    public async Task<StringResponse> Handle(GetTasksRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.data.ExtractPrinterId();

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.GetStoredMessageList.ToString(),
            cancellationToken, Array.Empty<Variant>());

        if (response is null)
        {
            _logger.LogError("Get tasks OPCUA call failed");

            throw new OPCUACommunicationFailedException("Get tasks OPCUA call failed");
        }

        var outputArguments = response.OutputArguments;
        var outputArgumentsLength = outputArguments?.Length;

        if (outputArgumentsLength != ExpectedOutputArgsLength)
        {
            _logger.LogError(
                "Output argument is not correct length. Expected {ExpectedOutputArgsLength}, got {outputArgumentsLength}",
                ExpectedOutputArgsLength, outputArgumentsLength);

            throw new InvalidDataException("Output argument is not correct length");
        }

        return new StringResponse(CommandName, printerId, GetResponseData(response));
    }
}