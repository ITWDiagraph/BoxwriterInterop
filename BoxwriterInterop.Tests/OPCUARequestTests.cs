namespace BoxwriterResmarkInterop.Tests;

using OPCUA;

using Workstation.ServiceModel.Ua;

public class OPCUARequestTests
{
    [Fact]
    public void GetArgsAsVariant_ReturnsExpectedResult()
    {
        var request = new OPCUARequest
        {
            PrinterId = "Printer1",
            Method = OPCUAMethods.GetStoredMessageList,
            TaskNumber = 123,
            InputArgs = new object[] { "file.txt", 10 }
        };

        var result = request.GetArgsAsVariant();

        Assert.Equal(3, result.Length);
        Assert.Equal("Printer1", request.PrinterId);
        Assert.Equal(OPCUAMethods.GetStoredMessageList, request.Method);
        Assert.Equal(new Variant(request.TaskNumber.Value), result[0]);
        Assert.Equal(new Variant("file.txt"), result[1]);
        Assert.Equal(new Variant(10), result[2]);
    }
}