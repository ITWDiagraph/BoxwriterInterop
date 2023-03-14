namespace BoxwriterResmarkInterop.OPCUA;

using Exceptions;

using Interfaces;

using MediatR;

using Requests;
using Workstation.ServiceModel.Ua;

public class StartTaskCommandHandler : BaseCommandHandler, IRequestHandler<StartTaskRequest, StringResponse>
{
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

        var inputArgs = new[] { new Variant(messageName)};

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.PrintStoredMessage.ToString(),
            cancellationToken, inputArgs);

        if (response is null)
        {
            _logger.LogError("Start task OPCUA call failed");

            throw new OPCUACommunicationFailedException("Start task OPCUA call failed");
        }

        var result = response.StatusCode;

        return FormatResponse(result, printerId);
    }
}