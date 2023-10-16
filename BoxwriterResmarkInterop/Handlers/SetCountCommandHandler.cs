using BoxwriterResmarkInterop.Extensions;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.Requests;

using MediatR;

using Workstation.ServiceModel.Ua;

namespace BoxwriterResmarkInterop.Handlers;
public class SetCountCommandHandler : IRequestHandler<SetCountRequest, StringResponse>
{
    private readonly IOPCUAService _opcuaService;

    public SetCountCommandHandler(IOPCUAService opcuaService) => _opcuaService = opcuaService;

    public async Task<StringResponse> Handle(SetCountRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();
        var startCount = Convert.ToUInt64(request.Data.ExtractAdditionalParameter());

        var opcuaRequest = new OPCUARequest
        {
            PrinterId = printerId,
            Method = OPCUAMethods.SetMessageCount,
            InputArgs = new object[] { startCount }
        };

        var response = await _opcuaService.CallMethodAsync(opcuaRequest, cancellationToken);

        return new StringResponse(Constants.SetCount, printerId, GetResponseData(response));
    }

    private static bool GetResponseData(CallMethodResult result) => StatusCode.IsGood(result.StatusCode);
}