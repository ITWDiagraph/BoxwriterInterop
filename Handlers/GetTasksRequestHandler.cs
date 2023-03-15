namespace BoxwriterResmarkInterop.Handlers;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Exceptions;

using Extensions;

using Interfaces;

using OPCUA;

using Requests;

using MediatR;

using Workstation.ServiceModel.Ua;

internal class GetTasksRequestHandler : IRequestHandler<GetTasksRequest, StringResponse>
{
    private const string CommandName = "Get tasks";
    private const int ExpectedOutputArgsLength = 2;
    private readonly ILogger<GetTasksRequestHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public GetTasksRequestHandler(IOPCUAService opcuaService, ILogger<GetTasksRequestHandler> logger)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    private static IEnumerable<string> GetResponseData(CallMethodResult result) =>
        result.OutputArguments?[1].Value as string[] ?? new [] { Constants.NoMessages };

    public async Task<StringResponse> Handle(GetTasksRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.GetStoredMessageList.ToString(),
            cancellationToken);

        if (response?.OutputArguments is null)
        {
            _logger.LogError("Get tasks OPCUA call failed");

            throw new OPCUACommunicationFailedException("Get tasks OPCUA call failed");
        }

        if (response.OutputArguments.Length != ExpectedOutputArgsLength)
        {
            _logger.LogError(
                "Output argument is not correct length. Expected {ExpectedOutputArgsLength}, got {outputArgumentsLength}",
                ExpectedOutputArgsLength, response.OutputArguments.Length);

            throw new InvalidDataException("Output argument is not correct length");
        }

        return new StringResponse(CommandName, printerId, GetResponseData(response));
    }
}