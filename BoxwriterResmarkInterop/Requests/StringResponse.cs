
namespace BoxwriterResmarkInterop.Requests;
public class StringResponse
{
    public StringResponse(string commandName, string printerId, string responseData) =>
        Data = FormatResponse(commandName, printerId, responseData);

    public StringResponse(string commandName, IEnumerable<string> responseData) =>
        Data = FormatResponse(commandName, responseData);

    public StringResponse(string commandName, string printerId, IEnumerable<string> responseData) =>
        Data = FormatResponse(commandName, printerId, responseData);

    public StringResponse(string commandName, string printerId, bool responseData) =>
        Data = FormatResponse(commandName, printerId, responseData ? "1" : "0");

    public string Data { get; }

    private static string FormatResponse(string commandName, string printerId, string responseData) =>
        FormatResponse(commandName, printerId, new[] { responseData });

    private static string FormatResponse(string commandName, string printerId, IEnumerable<string> responseData) =>
        FormatResponse(commandName, responseData.Prepend(printerId));

    private static string FormatResponse(string commandName, IEnumerable<string> responseData) =>
        $"{Constants.StartToken}{string.Join(Constants.TokenSeparator, responseData.Prepend(commandName))}{Constants.EndToken}";
}