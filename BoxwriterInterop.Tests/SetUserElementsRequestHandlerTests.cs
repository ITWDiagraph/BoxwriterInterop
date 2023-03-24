namespace BoxwriterResmarkInterop.Tests;

using OPCUA;

using Handlers;

using Interfaces;

using Requests;

using Workstation.ServiceModel.Ua;

public class SetUserElementsRequestHandlerTests
{
    private const string ValidRequest =
        "{Set user elements, 0000, Test Prompt 1, Test Value 1, Test Prompt 2, Test Value 2}";

    private const string InvalidMissingPairRequest = "{Set user elements, 0000, Test Prompt 1, Test Value 1, Test Prompt 2}";
    private const int TaskNumber = 1;
    private readonly AutoMocker _mocker = new();

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
        _mocker
            .GetMock<IOPCUAService>()
            .Setup(service => service.CallMethodAsync(
                It.Is<OPCUARequest>(request => request.Method == OPCUAMethods.SetMessageVariableData),
                It.IsAny<CancellationToken>()))
            .Returns(() =>
                Task.FromResult(new CallMethodResult { OutputArguments = new[] { new Variant(0) } }));

        var handler = _mocker.CreateInstance<SetUserElementsCommandHandler>();

        var response = await handler.Handle(new SetUserElementsRequest(ValidRequest), CancellationToken.None);

        Assert.Equal("{Get user elements, 0000, 2}", response.Data);
    }

    [Fact]
    public async Task SetUserElements_ThrowsOnBadRequestData()
    {
        var handler = _mocker.CreateInstance<SetUserElementsCommandHandler>();

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await handler.Handle(new SetUserElementsRequest(InvalidMissingPairRequest), CancellationToken.None));
    }
}