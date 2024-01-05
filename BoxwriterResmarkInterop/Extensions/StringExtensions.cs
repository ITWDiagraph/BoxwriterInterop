namespace BoxwriterResmarkInterop.Extensions;
public static class StringExtensions
{
    public static string ExtractPrinterId(this string data) => ExtractInputData(data)[1];

    private static string[] ExtractInputData(string data) => data.Trim(Constants.StartToken, Constants.EndToken).Split(Constants.TokenSeparator);

    public static string ExtractCommandName(this string data) => ExtractInputData(data)[0];

    public static string ExtractAdditionalParameter(this string data) => ExtractInputData(data)[2];
}