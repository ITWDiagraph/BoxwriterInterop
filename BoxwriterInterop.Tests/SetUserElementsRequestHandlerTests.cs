namespace BoxwriterResmarkInterop.Tests;

using Handlers;

using Interfaces;

using OPCUA;

using Requests;

using Workstation.ServiceModel.Ua;

public class SetUserElementsRequestHandlerTests
{
    private const string ValidRequest =
        "{Set user elements, 0000, Test Prompt 1, Test Value 1, Test Prompt 2, Test Value 2}";

    private const string InvalidRequest = "{Set user elements, 0000, Test Prompt 1, Test Value 1, Test Prompt 2}";
    private const int TaskNumber = 1;
    private readonly AutoMocker _mocker = new();

    public SetUserElementsRequestHandlerTests()
    {
        _mocker
            .GetMock<IOPCUAService>()
            .Setup(service => service.CallMethodAsync(
                It.IsAny<string>(),
                It.Is<string>(s => s == OPCUAMethods.SetMessageVariableData.ToString()),
                It.IsAny<CancellationToken>(),
                It.Is<int>(i => i == TaskNumber),
                It.IsAny<string>()))
            .Returns(() =>
                Task.FromResult(new CallMethodResult { OutputArguments = new[] { new Variant(0) } }));
    }

    private IOPCUAService _opcuaService => _mocker.GetMock<IOPCUAService>().Object;

    [Fact]
    public void SetUserElements_GetDataDictionary_ReturnsTrueIfEquivalent()
    {
        var data = SetUserElementsCommandHandler.GetDataAsDictionary(ValidRequest);

        Assert.Equivalent(new Dictionary<string, string>
            {
                { "Test Prompt 1", "Test Value 1" },
                { "Test Prompt 2", "Test Value 2" }
            },
            data, strict: true);
    }

    [Fact]
    public async Task SetUserElements_HandleGoodRequest_ReturnsValidStringResponse()
    {
        var handler = new SetUserElementsCommandHandler(_opcuaService);

        var response = await handler.Handle(new SetUserElementsRequest(ValidRequest), CancellationToken.None);

        Assert.Equal("{Get user elements, 0000, 2}", response.Data);
    }

    [Fact]
    public async Task SetUserElements_ThrowsOnBadRequestData()
    {
        var handler = new SetUserElementsCommandHandler(_opcuaService);

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await handler.Handle(new SetUserElementsRequest(InvalidRequest), CancellationToken.None));
    }
}