namespace BoxwriterResmarkInterop.OPCUA;

using Exceptions;

using Extensions;

using Interfaces;

using MediatR;

using Requests;

using Workstation.ServiceModel.Ua;

public class StartTaskCommandHandler : ICommandHandler, IRequestHandler<StartTaskRequest, StringResponse>
{
    private readonly ILogger<StartTaskCommandHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public StartTaskCommandHandler(ILogger<StartTaskCommandHandler> logger, IOPCUAService opcuaService)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    public string CommandName => "Start task";

    public string GetResponseData(CallMethodResult result)
    {
        return (StatusCode.IsGood(result.StatusCode) ? 1 : 0).ToString();
    }

    public async Task<StringResponse> Handle(StartTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.data.ExtractPrinterId();
        var messageName = request.data.ExtractMessageName();

        var inputArgs = new[] { new Variant(messageName) };

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.PrintStoredMessage.ToString(),
            cancellationToken, inputArgs);

        if (response is null)
        {
            _logger.LogError("Start task OPCUA call failed");

            throw new OPCUACommunicationFailedException("Start task OPCUA call failed");
        }

        return new StringResponse(CommandName, printerId, GetResponseData(response));
    }
}