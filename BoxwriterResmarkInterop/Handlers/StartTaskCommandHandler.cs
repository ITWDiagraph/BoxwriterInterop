namespace BoxwriterResmarkInterop.Handlers;

using Exceptions;

using Extensions;

using Interfaces;

using MediatR;

using OPCUA;

using Requests;

using Workstation.ServiceModel.Ua;

using static Constants;

public class StartTaskCommandHandler : IRequestHandler<StartTaskRequest, StringResponse>
{
    private readonly IOPCUAService _opcuaService;

    public StartTaskCommandHandler(IOPCUAService opcuaService)
    {
        _opcuaService = opcuaService;
    }

    public async Task<StringResponse> Handle(StartTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var response = await _opcuaService
            .CallMethodAsync(printerId, OPCUAMethods.ResumePrinting.ToString(), cancellationToken, TaskNumber)
            .ConfigureAwait(false);

        return new StringResponse(StartTask, printerId, GetResponseData(response));
    }

    private static bool GetResponseData(CallMethodResult result)
    {
        return StatusCode.IsGood(result.StatusCode);
    }
}