namespace BoxwriterResmarkInterop.Handlers
{
    using Exceptions;

    using Extensions;

    using Interfaces;

    using MediatR;

    using OPCUA;

    using Requests;

    using Workstation.ServiceModel.Ua;

    public class LoadTaskCommandHandler : IRequestHandler<LoadTaskRequest, StringResponse>
    {
        private const string CommandName = "Load task";
        private readonly ILogger<LoadTaskCommandHandler> _logger;
        private readonly IOPCUAService _opcuaService;

        public LoadTaskCommandHandler(ILogger<LoadTaskCommandHandler> logger, IOPCUAService opcuaService)
        {
            _logger = logger;
            _opcuaService = opcuaService;
        }

        public async Task<StringResponse> Handle(LoadTaskRequest request, CancellationToken cancellationToken)
        {
            var printerId = request.Data.ExtractPrinterId();
            var messageName = request.Data.ExtractMessageName();

            var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.PrintStoredMessage.ToString(), cancellationToken, messageName);

            if (response is null)
            {
                _logger.LogError("Start task OPCUA call failed");

                throw new OPCUACommunicationFailedException("Start task OPCUA call failed");
            }

            return new StringResponse(CommandName, printerId, GetResponseData(response));
        }

        private static bool GetResponseData(CallMethodResult result) => StatusCode.IsGood(result.StatusCode);
    }
}