using BoxwriterResmarkInterop.Extensions;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.Requests;

using MediatR;

using Workstation.ServiceModel.Ua;

namespace BoxwriterResmarkInterop.Handlers;
public class ResumeTaskCommandHandler : IRequestHandler<ResumeTaskRequest, StringResponse>
{
    private readonly IOPCUAService _opcuaService;

    public ResumeTaskCommandHandler(IOPCUAService opcuaService) => _opcuaService = opcuaService;

    public async Task<StringResponse> Handle(ResumeTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var opcuaRequest = new OPCUARequest
        {
            PrinterId = printerId,
            Method = OPCUAMethods.ResumePrinting,
            TaskNumber = Constants.TaskNumber
        };

        var response = await _opcuaService.CallMethodAsync(opcuaRequest, cancellationToken);

        return new StringResponse(Constants.ResumeTask, printerId, GetResponseData(response));
    }

    private static bool GetResponseData(CallMethodResult result) => StatusCode.IsGood(result.StatusCode);
}