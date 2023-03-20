namespace BoxwriterResmarkInterop.Tests;

using Handlers;

using Interfaces;

using OPCUA;

using Requests;

using Workstation.ServiceModel.Ua;

using XSerializer;

public class GetUserElementsRequestHandlerTests
{
    private const string ValidRequest = "{Get user elements, 0000}";
    private readonly AutoMocker _mocker = new();

    public GetUserElementsRequestHandlerTests()
    {
        _mocker.GetMock<IOPCUAService>().Setup(service => service.CallMethodAsync(
                It.IsAny<string>(),
                It.Is<string>(s => s == OPCUAMethods.GetMessageVariableData.ToString()),
                It.IsAny<CancellationToken>(),
                It.Is<int>(i => i == 1)))
            .Returns(() =>
            {
                var serializer = new XmlSerializer<Dictionary<string, string>>();

                var data = serializer.Serialize(new Dictionary<string, string>
                {
                    { "Test Prompt 1", "Test Value 1" },
                    { "Test Prompt 2", "Test Value 2" }
                });

                var result = new CallMethodResult { OutputArguments = new[] { new Variant(0), new Variant(data) } };

                return Task.FromResult(result);
            });
    }

    private IOPCUAService _opcuaService => _mocker.GetMock<IOPCUAService>().Object;

    private ILogger<GetUserElementsRequestHandler> _logger =>
        _mocker.GetMock<ILogger<GetUserElementsRequestHandler>>().Object;

    [Fact]
    public async Task GetUserElements_HandleGoodRequest_ReturnsValidStringResponse()
    {
        var handler = new GetUserElementsRequestHandler(_opcuaService, _logger);

        var response = await handler.Handle(new GetUserElementsRequest(ValidRequest), CancellationToken.None);

        Assert.Equal(response.Data,
            "{Get user elements, 0000, Test Prompt 1, Test Value 1, Test Prompt 2, Test Value 2}");
    }

    [Fact]
    public async Task GetUserElements_ThrowsOnNoData()
    {
        _mocker.GetMock<IOPCUAService>().Setup(service => service.CallMethodAsync(
                It.IsAny<string>(),
                It.Is<string>(s => s == OPCUAMethods.GetMessageVariableData.ToString()),
                It.IsAny<CancellationToken>(),
                It.Is<int>(i => i == 1)))
            .Returns(() => Task.FromResult(new CallMethodResult
            {
                OutputArguments = new[] { new Variant(0), new Variant(string.Empty) }
            }));

        var handler = new GetUserElementsRequestHandler(_opcuaService, _logger);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await handler.Handle(new GetUserElementsRequest(ValidRequest), CancellationToken.None));
    }
}