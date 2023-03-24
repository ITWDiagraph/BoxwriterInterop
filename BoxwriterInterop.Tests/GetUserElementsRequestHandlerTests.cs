namespace BoxwriterResmarkInterop.Tests;

using BoxwriterResmarkInterop.OPCUA;
using Handlers;

using Interfaces;

using OPCUA;

using Requests;

using Workstation.ServiceModel.Ua;

using XSerializer;

public class GetUserElementsRequestHandlerTests
{
    private const string ValidRequest = "{Get user elements, 0000}";
    private const int TaskNumber = 1;
    private readonly AutoMocker _mocker = new();

    [Fact]
    public async Task GetUserElements_HandleGoodRequest_ReturnsValidStringResponse()
    {
        _mocker
            .GetMock<IOPCUAService>()
            .Setup(service => service.CallMethodAsync(
                It.Is<OPCUARequest>(request => request.Method == OPCUAMethods.GetMessageVariableData),
                It.IsAny<CancellationToken>()))
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

        var handler = _mocker.CreateInstance<GetUserElementsRequestHandler>();

        var response = await handler.Handle(new GetUserElementsRequest(ValidRequest), CancellationToken.None);

        Assert.Equal("{Get user elements, 0000, Test Prompt 1, Test Value 1, Test Prompt 2, Test Value 2}", response.Data);
    }

    [Fact]
    public async Task GetUserElements_ThrowsOnNoData()
    {
        _mocker
            .GetMock<IOPCUAService>()
            .Setup(service => service.CallMethodAsync(
                It.Is<OPCUARequest>(request => request.Method == OPCUAMethods.GetMessageVariableData),
                It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(new CallMethodResult
            {
                OutputArguments = new[] { new Variant(0), new Variant(string.Empty) }
            }));

        var handler = _mocker.CreateInstance<GetUserElementsRequestHandler>();

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await handler.Handle(new GetUserElementsRequest(ValidRequest), CancellationToken.None));
    }
}