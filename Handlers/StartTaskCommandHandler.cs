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
    private readonly ILogger<StartTaskCommandHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public StartTaskCommandHandler(ILogger<StartTaskCommandHandler> logger, IOPCUAService opcuaService)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    public async Task<StringResponse> Handle(StartTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var response = await _opcuaService
            .CallMethodAsync(printerId, OPCUAMethods.ResumePrinting.ToString(), cancellationToken, TaskNumber)
            .ConfigureAwait(false);

        if (response is null)
        {
            _logger.LogError("Start task OPCUA call failed");

            throw new OPCUACommunicationFailedException("Start task OPCUA call failed");
        }

        return new StringResponse(StartTask, printerId, GetResponseData(response));
    }

    private static bool GetResponseData(CallMethodResult result)
    {
        return StatusCode.IsGood(result.StatusCode);
    }
}