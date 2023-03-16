namespace BoxwriterResmarkInterop.Handlers;

using Extensions;

using Interfaces;

using MediatR;

using OPCUA;

using Requests;

using XSerializer;

using static Constants;

using MoreLinq;

public class SetUserElementsCommandHandler : IRequestHandler<SetUserElementsRequest, StringResponse>
{
    private readonly IOPCUAService _opcuaService;

    public SetUserElementsCommandHandler(IOPCUAService opcuaService)
    {
        _opcuaService = opcuaService;
    }

    public async Task<StringResponse> Handle(SetUserElementsRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var data = request.Data.Split(TokenSeparator)
            .Skip(2)
            .Batch(2)
            .Select(t =>
            {
                var items = t.ToList();

                return new KeyValuePair<string, string>(items[0], items[1]);
            })
            .ToDictionary();

        var serializer = new XmlSerializer<Dictionary<string, string>>();

        _ = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.SetMessageVariableData.ToString(),
            cancellationToken, TaskNumber, serializer.Serialize(data)).ConfigureAwait(false);

        return new StringResponse(GetUserElements, printerId, data.Count.ToString());
    }
}