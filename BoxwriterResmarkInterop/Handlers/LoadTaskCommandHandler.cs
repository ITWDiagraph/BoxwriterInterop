using BoxwriterResmarkInterop.Extensions;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.Requests;

using MediatR;

using Workstation.ServiceModel.Ua;

namespace BoxwriterResmarkInterop.Handlers;
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

        return new StringResponse(Constants.LoadTask, printerId, GetResponseData(response));
    }

    private static bool GetResponseData(CallMethodResult result) => StatusCode.IsGood(result.StatusCode);
}