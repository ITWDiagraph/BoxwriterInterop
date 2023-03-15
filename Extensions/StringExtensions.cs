namespace BoxwriterResmarkInterop.Extensions;

using static Constants;

public static class StringExtensions
{
    public static string ExtractPrinterId(this string data)
    {
        return ExtractInputData(data)[1];
    }

    private static string[] ExtractInputData(string data)
    {
        data = data.Trim(StartToken);
        data = data.Trim(EndToken);

        var printerId = data.Split(TokenSeparator);

        return printerId;
    }

    public static string ExtractMessageName(this string data)
    {
        return ExtractInputData(data)[2];
    }
}