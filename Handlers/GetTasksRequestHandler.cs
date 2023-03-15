namespace BoxwriterResmarkInterop.Handlers;

using Exceptions;

using Extensions;

using Interfaces;

using MediatR;

using OPCUA;

using Requests;

using Workstation.ServiceModel.Ua;

using static Constants;

internal class GetTasksRequestHandler : IRequestHandler<GetTasksRequest, StringResponse>
{
    private const int ExpectedOutputArgsLength = 2;
    private readonly ILogger<GetTasksRequestHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public GetTasksRequestHandler(IOPCUAService opcuaService, ILogger<GetTasksRequestHandler> logger)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    public async Task<StringResponse> Handle(GetTasksRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.GetStoredMessageList.ToString(),
            cancellationToken).ConfigureAwait(false);

        if (response?.OutputArguments is null)
        {
            _logger.LogError("Get tasks OPCUA call failed");

            throw new OPCUACommunicationFailedException("Get tasks OPCUA call failed");
        }

        if (response.OutputArguments.Length != ExpectedOutputArgsLength)
        {
            _logger.LogError(
                "Output argument is not correct length. Expected {ExpectedOutputArgsLength}, got {outputArgumentsLength}",
                ExpectedOutputArgsLength, response.OutputArguments.Length);

            throw new InvalidDataException("Output argument is not correct length");
        }

        return new StringResponse(GetTasks, printerId, GetResponseData(response));
    }

    private static IEnumerable<string> GetResponseData(CallMethodResult result)
    {
        return result.OutputArguments?[1].Value as string[] ?? new[] { NoMessages };
    }
}