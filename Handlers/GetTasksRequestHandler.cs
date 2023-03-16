namespace BoxwriterResmarkInterop.Handlers;

using Extensions;

using Interfaces;

using MediatR;

using OPCUA;

using Requests;

using Workstation.ServiceModel.Ua;

using static Constants;

internal class GetTasksRequestHandler : IRequestHandler<GetTasksRequest, StringResponse>
{
    private readonly IOPCUAService _opcuaService;

    public GetTasksRequestHandler(IOPCUAService opcuaService)
    {
        _opcuaService = opcuaService;
    }

    public async Task<StringResponse> Handle(GetTasksRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.GetStoredMessageList.ToString(),
            cancellationToken).ConfigureAwait(false);

        return new StringResponse(GetTasks, printerId, GetResponseData(response));
    }

    private static IEnumerable<string> GetResponseData(CallMethodResult result)
    {
        return result.OutputArguments?[1].Value as string[] ?? new[] { NoMessages };
    }
}