namespace BoxwriterResmarkInterop.Handlers;

using Exceptions;

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

    public LoadTaskCommandHandler(IOPCUAService opcuaService)
    {
        _opcuaService = opcuaService;
    }

    public async Task<StringResponse> Handle(LoadTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();
        var messageName = request.Data.ExtractAdditionalParameter();

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.PrintStoredMessage.ToString(), cancellationToken, messageName);

        return new StringResponse(LoadTask, printerId, GetResponseData(response));
    }

    private static bool GetResponseData(CallMethodResult result) => StatusCode.IsGood(result.StatusCode);
}