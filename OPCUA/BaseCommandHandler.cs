namespace BoxwriterResmarkInterop.OPCUA;

using System.Text.RegularExpressions;

using Exceptions;

using Requests;

using Workstation.ServiceModel.Ua;

public abstract class BaseCommandHandler
{
    protected const char StartToken = '{';
    protected const char EndToken = '}';
    protected const string TokenSeparator = ", ";
    private readonly ILogger<BaseCommandHandler> _logger;

    protected BaseCommandHandler(ILogger<BaseCommandHandler> logger)
    {
        _logger = logger;
    }

    protected abstract StringResponse FormatResponse(Variant[]? outputArguments, string? printerId);

    protected string ExtractPrinterId(string data)
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
}   