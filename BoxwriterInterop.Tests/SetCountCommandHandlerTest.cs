using BoxwriterResmarkInterop.Handlers;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.Requests;

using Workstation.ServiceModel.Ua;

namespace BoxwriterResmarkInterop.Tests;
public class SetCountCommandHandlerTest
{
    private const string ValidRequest = "{Set count, 0000, 5}";
    private const string ValidResponse = "{Set count, 0000, 1}";
    private const string InvalidRequest = "{Set count, 0000}";
    private readonly AutoMocker _mocker = new();

    [Fact]
    public async Task SetMessageCount_HandleGoodRequest_ReturnsValidStringResponse()
    {
        _mocker
            .GetMock<IOPCUAService>()
            .Setup(service => service.CallMethodAsync(
                It.Is<OPCUARequest>(request => request.Method == OPCUAMethods.SetMessageCount),
                It.IsAny<CancellationToken>()))
            .Returns(() =>
                Task.FromResult(new CallMethodResult { OutputArguments = new[] { new Variant(0) } }));

        var handler = _mocker.CreateInstance<SetCountCommandHandler>();

        var response = await handler.Handle(new SetCountRequest(ValidRequest), CancellationToken.None);

        Assert.Equal(ValidResponse, response.Data);
    }

    [Fact]
    public async Task SetUserElements_ThrowsOnBadRequestData()
    {
        var handler = _mocker.CreateInstance<SetCountCommandHandler>();

        await Assert.ThrowsAsync<IndexOutOfRangeException>(async () =>
            await handler.Handle(new SetCountRequest(InvalidRequest), CancellationToken.None));
    }
}