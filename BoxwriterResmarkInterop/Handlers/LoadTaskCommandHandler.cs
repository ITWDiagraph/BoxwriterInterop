namespace BoxwriterResmarkInterop.Handlers;

using Extensions;

using Interfaces;

using MediatR;

using OPCUA;

using Requests;

using Workstation.ServiceModel.Ua;

using static Constants;

public class LoadTaskCommandHandler : IRequestHandler<LoadTaskRequest, StringResponse>
{
    private readonly IOPCUAService _opcuaService;
    private readonly ILogger<LoadTaskCommandHandler> _logger;

    public LoadTaskCommandHandler(IOPCUAService opcuaService, ILogger<LoadTaskCommandHandler> logger)
    {
        _opcuaService = opcuaService;
        _logger = logger;
    }

    public async Task<StringResponse> Handle(LoadTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();
        var messageName = request.Data.ExtractAdditionalParameter();

        if (messageName.StartsWith(" ") || messageName.EndsWith(" "))
        {
            _logger.LogWarning("Message name \"{MessageName}\" has leading or trailing spaces", messageName);
        }

        var opcuaRequest = new OPCUARequest
        {
            PrinterId = printerId,
            Method = OPCUAMethods.PrintStoredMessage,
            InputArgs = new object[] { messageName }
        };

        var response = await _opcuaService.CallMethodAsync(opcuaRequest, cancellationToken);

        return new StringResponse(LoadTask, printerId, GetResponseData(response));
    }

    private static bool GetResponseData(CallMethodResult result) => StatusCode.IsGood(result.StatusCode);
}