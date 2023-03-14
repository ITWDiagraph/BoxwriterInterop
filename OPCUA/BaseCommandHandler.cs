namespace BoxwriterResmarkInterop.OPCUA;

using System.Text;
using System.Text.RegularExpressions;

using Exceptions;

using Requests;

using Workstation.ServiceModel.Ua;

public abstract class BaseCommandHandler
{
    private const char StartToken = '{';
    private const char EndToken = '}';
    private const string TokenSeparator = ", ";
    private readonly ILogger<BaseCommandHandler> _logger;

    protected BaseCommandHandler(ILogger<BaseCommandHandler> logger)
    {
        _logger = logger;
    }

    protected abstract string CommandName { get; }

    protected virtual StringResponse FormatResponse(Variant[]? outputArguments, string? printerId)
    {
        var data = Enumerable.Empty<string>()
            .Append(CommandName)
            .Append(printerId)
            .Concat(GetResponseData(outputArguments));

        return new StringResponse(
            $"{StartToken}{string.Join(TokenSeparator, data)}{EndToken}");
    }

    protected virtual StringResponse FormatResponse(StatusCode result, string? printerId)
    {
        var data = Enumerable.Empty<string>()
            .Append(CommandName)
            .Append(printerId)
            .Append(ParseStatusCode(result).ToString());

        return new StringResponse(
            $"{StartToken}{string.Join(TokenSeparator, data)}{EndToken}");
    }

    private static int ParseStatusCode(StatusCode statusCode)
    {
        return StatusCode.IsGood(statusCode.Value) ? 1 : 0;
    }

    protected virtual IEnumerable<string> GetResponseData(Variant[]? outputArguments)
    {
        throw new NotImplementedException();
    }

    protected static string ExtractPrinterId(string data)
    {
        data = data.Trim(StartToken);
        data = data.Trim(EndToken);

        var printerId = data.Split(TokenSeparator)[1];

        if (printerId is null)
        {
            throw new PrinterNotFoundException("Could not extract printer ID");
        }

        return printerId;
    }

    protected static string ExtractMessageName(string data)
    {
        data = data.Trim(StartToken);
        data = data.Trim(EndToken);

        var messageName = data.Split(TokenSeparator)[2];

        if (messageName is null)
        {
            throw new NullReferenceException("Could not extract message name");
        }

        return messageName;
    }
}   