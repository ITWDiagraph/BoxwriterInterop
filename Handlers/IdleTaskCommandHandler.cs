namespace BoxwriterResmarkInterop.Handlers;

using Exceptions;

using Extensions;

using Interfaces;

using MediatR;

using OPCUA;

using Requests;

using Workstation.ServiceModel.Ua;

using static Constants;

public class IdleTaskCommandHandler : IRequestHandler<IdleTaskRequest, StringResponse>
{
    private readonly IOPCUAService _opcuaService;

    public IdleTaskCommandHandler(IOPCUAService opcuaService)
    {
        _opcuaService = opcuaService;
    }

    public async Task<StringResponse> Handle(IdleTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.StopPrinting.ToString(), cancellationToken, TaskNumber)
            .ConfigureAwait(false);

        return new StringResponse(IdleTask, printerId, GetResponseData(response));
    }

    private static bool GetResponseData(CallMethodResult result) => StatusCode.IsGood(result.StatusCode);
}