namespace BoxwriterResmarkInterop.Handlers;

using Exceptions;

using Extensions;

using Interfaces;

using MediatR;

using OPCUA;

using Requests;

using Workstation.ServiceModel.Ua;

using static Constants;

public class ResumeTaskCommandHandler : IRequestHandler<ResumeTaskRequest, StringResponse>
{
    private readonly ILogger<ResumeTaskCommandHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public ResumeTaskCommandHandler(ILogger<ResumeTaskCommandHandler> logger, IOPCUAService opcuaService)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    public async Task<StringResponse> Handle(ResumeTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.ResumePrinting.ToString(), cancellationToken, TaskNumber)
            .ConfigureAwait(false);

        if (response is null)
        {
            _logger.LogError("{ResumeTask} OPCUA call failed", ResumeTask);

            throw new OPCUACommunicationFailedException($"{ResumeTask} OPCUA call failed");
        }

        return new StringResponse(ResumeTask, printerId, GetResponseData(response));
    }

    private static bool GetResponseData(CallMethodResult result) => StatusCode.IsGood(result.StatusCode);
}