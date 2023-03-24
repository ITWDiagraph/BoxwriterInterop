namespace BoxwriterResmarkInterop.Handlers;

using Extensions;

using Interfaces;

using MediatR;

using OPCUA;

using Requests;

using Workstation.ServiceModel.Ua;

using static Constants;

public class GetTasksRequestHandler : IRequestHandler<GetTasksRequest, StringResponse>
{
    private readonly IOPCUAService _opcuaService;

    public GetTasksRequestHandler(IOPCUAService opcuaService)
    {
        _opcuaService = opcuaService;
    }

    public async Task<StringResponse> Handle(GetTasksRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var opcuaRequest = new OPCUARequest
        {
            PrinterId = printerId,
            Method = OPCUAMethods.GetStoredMessageList
        };

        var response = await _opcuaService.CallMethodAsync(opcuaRequest, cancellationToken);

        return new StringResponse(GetTasks, printerId, GetResponseData(response));
    }

    private static IEnumerable<string> GetResponseData(CallMethodResult result)
    {
        var data = result.OutputArguments?[1].Value as IEnumerable<string>;

        return data ?? Enumerable.Empty<string>();
    }
}