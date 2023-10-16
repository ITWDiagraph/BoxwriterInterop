using BoxwriterResmarkInterop.Extensions;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.Requests;

using MediatR;

using Workstation.ServiceModel.Ua;

using XSerializer;

namespace BoxwriterResmarkInterop.Handlers;
public class GetUserElementsRequestHandler : IRequestHandler<GetUserElementsRequest, StringResponse>
{
    private readonly ILogger<GetUserElementsRequestHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public GetUserElementsRequestHandler(IOPCUAService opcuaService, ILogger<GetUserElementsRequestHandler> logger)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    public async Task<StringResponse> Handle(GetUserElementsRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var opcuaRequest = new OPCUARequest
        {
            PrinterId = printerId,
            Method = OPCUAMethods.GetMessageVariableData,
            TaskNumber = Constants.TaskNumber
        };

        var response = await _opcuaService.CallMethodAsync(opcuaRequest, cancellationToken);

        return new StringResponse(Constants.GetUserElements, printerId, GetResponseData(response));
    }

    private IEnumerable<string> GetResponseData(CallMethodResult result)
    {
        var data = result.OutputArguments?[1].Value as string;

        if (string.IsNullOrWhiteSpace(data))
        {
            throw new InvalidOperationException("Task is not loaded, no data returned");
        }

        _logger.LogDebug("Got variable data {Data}", data);

        var serializer = new XmlSerializer<Dictionary<string, string>>();

        return serializer.Deserialize(data).SelectMany(pair => new[] { pair.Key, pair.Value });
    }
}