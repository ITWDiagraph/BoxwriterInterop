using BoxwriterResmarkInterop.Extensions;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.Requests;

using MediatR;

using MoreLinq;

using XSerializer;

namespace BoxwriterResmarkInterop.Handlers;
public class SetUserElementsCommandHandler : IRequestHandler<SetUserElementsRequest, StringResponse>
{
    private readonly IOPCUAService _opcuaService;

    public SetUserElementsCommandHandler(IOPCUAService opcuaService) => _opcuaService = opcuaService;

    public async Task<StringResponse> Handle(SetUserElementsRequest request, CancellationToken cancellationToken)
    {
        var data = GetDataAsDictionary(request.Data);

        var serializer = new XmlSerializer<Dictionary<string, string>>();

        var printerId = request.Data.ExtractPrinterId();

        var opcuaRequest = new OPCUARequest
        {
            PrinterId = printerId,
            Method = OPCUAMethods.SetMessageVariableData,
            TaskNumber = Constants.TaskNumber,
            InputArgs = new object[] { serializer.Serialize(data) }
        };

        _ = await _opcuaService.CallMethodAsync(opcuaRequest, cancellationToken);

        return new StringResponse(Constants.GetUserElements, printerId, data.Count.ToString());
    }

    public static Dictionary<string, string> GetDataAsDictionary(string data) =>
        data
            .Trim(Constants.StartToken, Constants.EndToken)
            .Split(Constants.TokenSeparator)
            .Skip(2)
            .Batch(2)
            .Select(t =>
            {
                var items = t.ToList();

                return new KeyValuePair<string, string>(items[0], items[1]);
            })
            .ToDictionary();
}