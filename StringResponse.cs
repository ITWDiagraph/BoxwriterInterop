namespace BoxwriterResmarkInterop;

using static Constants;

public class StringResponse
{
    public StringResponse(string commandName, string printerId, string responseData)
    {
        Data = FormatResponse(commandName, printerId, responseData);
    }

    public string Data { get; set; }

    private string FormatResponse(string commandName, string? printerId, string responseData)
    {
        var data = Enumerable.Empty<string>()
            .Append(commandName)
            .Append(printerId)
            .Append(responseData);

        return $"{StartToken}{string.Join(TokenSeparator, data)}{EndToken}";
    }
}