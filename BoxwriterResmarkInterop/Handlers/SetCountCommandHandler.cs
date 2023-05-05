namespace BoxwriterResmarkInterop.Handlers;

using Extensions;

using Interfaces;

using MediatR;

using OPCUA;

using Requests;

using Workstation.ServiceModel.Ua;

using static Constants;

public class SetCountCommandHandler : IRequestHandler<SetCountRequest, StringResponse>
{
    private readonly IOPCUAService _opcuaService;

    public SetCountCommandHandler(IOPCUAService opcuaService)
    {
        _opcuaService = opcuaService;
    }

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

        return new StringResponse(SetCount, printerId, GetResponseData(response));
    }

    private static bool GetResponseData(CallMethodResult result)
    {
        return StatusCode.IsGood(result.StatusCode);
    }
}