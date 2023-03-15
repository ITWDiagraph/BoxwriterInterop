namespace BoxwriterResmarkInterop.Requests;

using static Constants;

public class StringResponse
{
    public StringResponse(string commandName, string printerId, string responseData)
    {
        Data = FormatResponse(commandName, printerId, responseData);
    }

    public string Data { get; set; }

    private static string FormatResponse(string commandName, string printerId, string responseData)
    {
        return $"{StartToken}{string.Join(TokenSeparator, commandName, printerId, responseData)}{EndToken}";
    }
}