namespace BoxwriterResmarkInterop.Handlers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoxwriterResmarkInterop.Exceptions;
using BoxwriterResmarkInterop.Extensions;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.Requests;
using MediatR;
using Workstation.ServiceModel.Ua;

internal class GetTasksRequestHandler : IRequestHandler<GetTasksRequest, StringResponse>
{
    private const string CommandName = "Get tasks";
    private const int ExpectedOutputArgsLength = 2;
    private const string NoMessages = "NoMessages";
    private readonly ILogger<GetTasksRequestHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public GetTasksRequestHandler(IOPCUAService opcuaService, ILogger<GetTasksRequestHandler> logger)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    private static string GetResponseData(CallMethodResult result)
    {
        return result.OutputArguments?[1].Value is string[] args
            ? args.Aggregate((current, arg) => current + Constants.TokenSeparator + arg)
            : NoMessages;
    }

    public async Task<StringResponse> Handle(GetTasksRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.GetStoredMessageList.ToString(),
            cancellationToken, Array.Empty<Variant>());

        if (response is null || response.OutputArguments is null)
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
