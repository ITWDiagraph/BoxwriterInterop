namespace BoxwriterResmarkInterop.Extensions;

using static Constants;

public static class StringExtensions
{
    public static string ExtractPrinterId(this string data)
    {
        data = data.Trim(StartToken);
        data = data.Trim(EndToken);

        var printerId = data.Split(TokenSeparator)[1];

        return printerId;
    }

    public static string ExtractMessageName(this string data)
    {
        data = data.Trim(StartToken);
        data = data.Trim(EndToken);

        var messageName = data.Split(TokenSeparator)[2];

        return messageName;
    }
}