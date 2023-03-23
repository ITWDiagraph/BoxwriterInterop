namespace BoxwriterResmarkInterop.Requests;

using static Constants;

public class StringResponse
{
    public StringResponse(string commandName, string printerId, string responseData)
    {
        Data = FormatResponse(commandName, printerId, responseData);
    }

    public StringResponse(string commandName, IEnumerable<string> responseData)
    {
        Data = FormatResponse(commandName, responseData);
    }

    public StringResponse(string commandName, string printerId, IEnumerable<string> responseData)
    {
        Data = FormatResponse(commandName, printerId, responseData);
    }

    public StringResponse(string commandName, string printerId, bool responseData)
    {
        Data = FormatResponse(commandName, printerId, responseData ? "1" : "0");
    }

    public string Data { get; }

    private static string FormatResponse(string commandName, string printerId, string responseData)
    {
        return FormatResponse(commandName, printerId, new[] { responseData });
    }

    private static string FormatResponse(string commandName, string printerId, IEnumerable<string> responseData)
    {
        var data = Enumerable.Empty<string>()
            .Append(printerId)
            .Concat(responseData);

        return FormatResponse(commandName, data);
    }

    private static string FormatResponse(string commandName, IEnumerable<string> responseData)
    {
        var data = Enumerable.Empty<string>()
            .Append(commandName)
            .Concat(responseData);

        return $"{StartToken}{string.Join(TokenSeparator, data)}{EndToken}";
    }
}