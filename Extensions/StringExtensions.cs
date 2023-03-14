namespace BoxwriterResmarkInterop.Extensions;

using Exceptions;

using static Constants;

public static class StringExtensions
{
    public static string ExtractPrinterId(this string data)
    {
        data = data.Trim(StartToken);
        data = data.Trim(EndToken);

        var printerId = data.Split(TokenSeparator)[1] ??
                        throw new PrinterNotFoundException("Could not extract printer ID");

        return printerId;
    }

    public static string ExtractMessageName(this string data)
    {
        data = data.Trim(StartToken);
        data = data.Trim(EndToken);

        var messageName = data.Split(TokenSeparator)[2] ??
                          throw new NullReferenceException("Could not extract message name");

        return messageName;
    }
}