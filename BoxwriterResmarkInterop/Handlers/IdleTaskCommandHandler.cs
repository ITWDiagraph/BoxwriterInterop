using BoxwriterResmarkInterop.Extensions;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.Requests;

using MediatR;

using Workstation.ServiceModel.Ua;

namespace BoxwriterResmarkInterop.Handlers;
public class IdleTaskCommandHandler : IRequestHandler<IdleTaskRequest, StringResponse>
{
    private readonly IOPCUAService _opcuaService;

    public IdleTaskCommandHandler(IOPCUAService opcuaService) => _opcuaService = opcuaService;

    public async Task<StringResponse> Handle(IdleTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var opcuaRequest = new OPCUARequest
        {
            PrinterId = printerId,
            Method = OPCUAMethods.StopPrinting,
            TaskNumber = Constants.TaskNumber
        };

        var response = await _opcuaService.CallMethodAsync(opcuaRequest, cancellationToken);

        return new StringResponse(Constants.IdleTask, printerId, GetResponseData(response));
    }

    private static bool GetResponseData(CallMethodResult result) => StatusCode.IsGood(result.StatusCode);
}