using BoxwriterResmarkInterop.Extensions;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.Requests;

using MediatR;

using Workstation.ServiceModel.Ua;

namespace BoxwriterResmarkInterop.Handlers;
public class GetTasksRequestHandler : IRequestHandler<GetTasksRequest, StringResponse>
{
    private readonly IOPCUAService _opcuaService;

    public GetTasksRequestHandler(IOPCUAService opcuaService) => _opcuaService = opcuaService;

    public async Task<StringResponse> Handle(GetTasksRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var opcuaRequest = new OPCUARequest
        {
            PrinterId = printerId,
            Method = OPCUAMethods.GetStoredMessageList
        };

        var response = await _opcuaService.CallMethodAsync(opcuaRequest, cancellationToken);

        return new StringResponse(Constants.GetTasks, printerId, GetResponseData(response));
    }

    private static IEnumerable<string> GetResponseData(CallMethodResult result)
    {
        var data = result.OutputArguments?[1].Value as IEnumerable<string>;

        return data ?? Enumerable.Empty<string>();
    }
}