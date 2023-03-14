namespace BoxwriterResmarkInterop.OPCUA;

using Exceptions;

using Interfaces;

using MediatR;

using Requests;

using Workstation.ServiceModel.Ua;

public class GetTaskCommandHandler : BaseCommandHandler, IRequestHandler<GetTasksRequest, StringResponse>
{
    private const int ExpectedOutputArgsLength = 2;
    private const string NoMessages = "NoMessages";
    private readonly ILogger<GetTaskCommandHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public GetTaskCommandHandler(ILogger<GetTaskCommandHandler> logger, IOPCUAService opcuaService)
        : base(logger)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    protected override string CommandName => "Get tasks";

    public async Task<StringResponse> Handle(GetTasksRequest request, CancellationToken cancellationToken)
    {
        var printerId = ExtractPrinterId(request.data);

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

        return FormatResponse(outputArguments, printerId);
    }

    protected override IEnumerable<string> GetResponseData(Variant[]? outputArguments)
    {
        if (outputArguments?[1].Value is string[] args)
        {
            return args;
        }

        return new[] { NoMessages };
    }
}