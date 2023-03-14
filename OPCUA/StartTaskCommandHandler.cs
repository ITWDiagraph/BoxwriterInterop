namespace BoxwriterResmarkInterop.OPCUA;

using Exceptions;

using Interfaces;

using MediatR;

using Requests;

using Workstation.ServiceModel.Ua;

public class StartTaskCommandHandler : BaseCommandHandler, IRequestHandler<StartTaskRequest, StringResponse>
{
    private const int ExpectedOutputArgsLength = 2;
    private readonly ILogger<BaseCommandHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public StartTaskCommandHandler(ILogger<StartTaskCommandHandler> logger, IOPCUAService opcuaService)
        : base(logger)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    protected override string CommandName => "Start task";

    public async Task<StringResponse> Handle(StartTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = ExtractPrinterId(request.data);
        var messageName = ExtractMessageName(request.data);

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.PrintStoredMessage.ToString(),
            cancellationToken, messageName);

        if (response is null)
        {
            _logger.LogError("Start task OPCUA call failed");

            throw new OPCUACommunicationFailedException("Start task OPCUA call failed");
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

        return Enumerable.Empty<string>();
    }
}