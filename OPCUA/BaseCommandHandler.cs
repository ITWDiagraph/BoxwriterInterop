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

    protected abstract IEnumerable<string> GetResponseData(Variant[]? outputArguments);

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