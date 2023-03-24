namespace BoxwriterResmarkInterop.Handlers;

using Extensions;

using Interfaces;

using MediatR;

using OPCUA;

using Requests;

using Workstation.ServiceModel.Ua;

using XSerializer;

using static Constants;

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
            TaskNumber = TaskNumber
        };

        var response = await _opcuaService.CallMethodAsync(opcuaRequest, cancellationToken).ConfigureAwait(false);

        return new StringResponse(GetUserElements, printerId, GetResponseData(response));
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