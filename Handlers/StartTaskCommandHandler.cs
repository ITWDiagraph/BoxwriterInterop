namespace BoxwriterResmarkInterop.Handlers;

using System.Threading.Tasks;

using BoxwriterResmarkInterop.Exceptions;
using BoxwriterResmarkInterop.Extensions;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.Requests;
using MediatR;
using Workstation.ServiceModel.Ua;

public class StartTaskCommandHandler : IRequestHandler<StartTaskRequest, StringResponse>
{
    private const string CommandName = "Start task";
    private readonly ILogger<StartTaskCommandHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public StartTaskCommandHandler(ILogger<StartTaskCommandHandler> logger, IOPCUAService opcuaService)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    private static string GetResponseData(CallMethodResult result) =>
        StatusCode.IsGood(result.StatusCode) ? "1" : "0";

    public async Task<StringResponse> Handle(StartTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();
        var messageName = request.Data.ExtractMessageName();

        var inputArgs = new[] { new Variant(messageName) };

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.PrintStoredMessage.ToString(),
            cancellationToken, inputArgs);

        if (response is null)
        {
            _logger.LogError("Start task OPCUA call failed");

            throw new OPCUACommunicationFailedException("Start task OPCUA call failed");
        }

        return new StringResponse(CommandName, printerId, GetResponseData(response));
    }
}