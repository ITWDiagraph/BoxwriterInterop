namespace BoxwriterResmarkInterop.Extensions;

using static Constants;

public static class StringExtensions
{
    public static string ExtractPrinterId(this string data) => ExtractInputData(data)[1];

    private static string[] ExtractInputData(string data) => data.Trim(StartToken, EndToken).Split(TokenSeparator);

    public static string ExtractCommandName(this string data) => ExtractInputData(data)[0];

    public static string ExtractMessageName(this string data) => ExtractInputData(data)[2];
}