namespace BoxwriterResmarkInterop.Handlers;

using Exceptions;

using Extensions;

using Interfaces;

using MediatR;

using OPCUA;

using Requests;

using Workstation.ServiceModel.Ua;

using static Constants;

public class GetLinesRequestHandler : IRequestHandler<GetLinesRequest, StringResponse>
{
    private const int ExpectedOutputArgsLength = 3;
    private readonly ILogger<GetLinesRequestHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public GetLinesRequestHandler(IOPCUAService opcuaService, ILogger<GetLinesRequestHandler> logger)
    {
        _opcuaService = opcuaService;
        _logger = logger;
    }

    public async Task<StringResponse> Handle(GetLinesRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.GetLines.ToString(), cancellationToken)
            .ConfigureAwait(false);

        if (response?.OutputArguments is null)
        {
            _logger.LogError("Get tasks OPCUA call failed");

            throw new OPCUACommunicationFailedException("Get tasks OPCUA call failed");
        }

        if (response.OutputArguments.Length != ExpectedOutputArgsLength)
        {
            _logger.LogError("Output argument is not correct length. Expected {ExpectedOutputArgsLength}, got {outputArgumentsLength}",
                ExpectedOutputArgsLength,
                response.OutputArguments.Length);

            throw new InvalidDataException("Output argument is not correct length");
        }

        return new StringResponse(GetLines, printerId, GetResponseData(response));
    }

    private static IEnumerable<string> GetResponseData(CallMethodResult result)
    {
        return result.OutputArguments?[1].Value as string[] ?? new[] { NoLines };
    }
}