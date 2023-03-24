namespace BoxwriterResmarkInterop.Handlers;

using Extensions;

using Interfaces;

using MediatR;

using MoreLinq;

using OPCUA;

using Requests;

using XSerializer;

using static Constants;

public class SetUserElementsCommandHandler : IRequestHandler<SetUserElementsRequest, StringResponse>
{
    private readonly IOPCUAService _opcuaService;

    public SetUserElementsCommandHandler(IOPCUAService opcuaService)
    {
        _opcuaService = opcuaService;
    }

    public async Task<StringResponse> Handle(SetUserElementsRequest request, CancellationToken cancellationToken)
    {
        var data = GetDataAsDictionary(request.Data);

        var serializer = new XmlSerializer<Dictionary<string, string>>();

        var printerId = request.Data.ExtractPrinterId();

        var opcuaRequest = new OPCUARequest
        {
            PrinterId = printerId,
            Method = OPCUAMethods.SetMessageVariableData,
            TaskNumber = TaskNumber,
            InputArgs = new object[] { serializer.Serialize(data) }
        };

        _ = await _opcuaService.CallMethodAsync(opcuaRequest, cancellationToken);

        return new StringResponse(GetUserElements, printerId, data.Count.ToString());
    }

    public static Dictionary<string, string> GetDataAsDictionary(string data) =>
        data
            .Trim(StartToken, EndToken)
            .Split(TokenSeparator)
            .Skip(2)
            .Batch(2)
            .Select(t =>
            {
                var items = t.ToList();

                return new KeyValuePair<string, string>(items[0], items[1]);
            })
            .ToDictionary();
}