using BoxwriterResmarkInterop.Exceptions;
using BoxwriterResmarkInterop.Extensions;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.Requests;
using MediatR;
using Workstation.ServiceModel.Ua;

namespace BoxwriterResmarkInterop.Handlers;

public class LoadTaskCommandHandler : IRequestHandler<LoadTaskRequest, StringResponse>
{
    private readonly ILogger<LoadTaskCommandHandler> _logger;
    private readonly IOPCUAService _opcuaService;
    private const string CommandName = "Load task";

    public LoadTaskCommandHandler(ILogger<LoadTaskCommandHandler> logger, IOPCUAService opcuaService)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    public async Task<StringResponse> Handle(LoadTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();
        var messageName = request.Data.ExtractMessageName();

        var response =
            await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.ResumePrinting.ToString(), cancellationToken, messageName);

        if (response is null)
        {
            _logger.LogError("Start task OPCUA call failed");

            throw new OPCUACommunicationFailedException("Start task OPCUA call failed");
        }

        return new StringResponse(CommandName, printerId, GetResponseData(response));
    }

    private static bool GetResponseData(CallMethodResult result)
    {
        return StatusCode.IsGood(result.StatusCode);
    }
}