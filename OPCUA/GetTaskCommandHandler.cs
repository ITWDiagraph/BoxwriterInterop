namespace BoxwriterResmarkInterop.OPCUA;

using System.Text;

using Exceptions;

using Interfaces;

using MediatR;

using Requests;

using Workstation.ServiceModel.Ua;

public class GetTaskCommandHandler : BaseCommandHandler, IRequestHandler<GetTaskRequest, StringResponse>
{
    private const int ExpectedOutputArgsLength = 2;
    private const string NoMessages = "NoMessages";
    private const string GetTasks = "Get tasks";
    private readonly ILogger<GetTaskCommandHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public GetTaskCommandHandler(ILogger<GetTaskCommandHandler> logger, IOPCUAService opcuaService)
        : base(logger)
    {
        _logger = logger;
        _opcuaService = opcuaService;
    }

    public async Task<StringResponse> Handle(GetTaskRequest request, CancellationToken cancellationToken)
    {
        var printerId = ExtractPrinterId(request.data);

        var response = await _opcuaService.CallMethodAsync(printerId, OPCUAMethods.GetStoredMessageList.ToString(), cancellationToken);

        if (response is null)
        {
            _logger.LogError("Get tasks OPCUA call failed");

            throw new OPCUACommunicationFailedException("Get tasks OPCUA call failed");
        }

        var outputArguments = response.OutputArguments;
        var outputArgumentsLength = outputArguments?.Length;

        if (outputArgumentsLength != ExpectedOutputArgsLength)
        {
            _logger.LogError(
                "Output argument is not correct length. Expected {ExpectedOutputArgsLength}, got {outputArgumentsLength}",
                ExpectedOutputArgsLength, outputArgumentsLength);

            throw new InvalidDataException("Output argument is not correct length");
        }

        return FormatResponse(outputArguments, printerId);
    }

    protected override StringResponse FormatResponse(Variant[]? outputArguments, string? printerId)
    {
        var responseData = string.Empty;

        if (outputArguments != null)
        {
            if (outputArguments[1].Value is string[] args)
            {
                var argsBuilder = new StringBuilder();

                foreach (var arg in args)
                {
                    argsBuilder.Append(arg + TokenSeparator);
                }

                responseData = argsBuilder.ToString();
            }
            else
            {
                responseData = NoMessages;
            }
        }

        return new StringResponse(
            $"{StartToken}{string.Join(TokenSeparator, GetTasks, printerId, responseData)}{EndToken}");
    }
}