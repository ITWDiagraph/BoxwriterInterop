
using BoxwriterResmarkInterop.Exceptions;
using BoxwriterResmarkInterop.OPCUA;

using Workstation.ServiceModel.Ua;

namespace BoxwriterResmarkInterop.Tests;
public class ResmarkOpcuaServiceTests
{
    private readonly AutoMocker _mocker = new();

    [Fact]
    public async Task CallMethodAsync_ThrowsPrinterNotFoundException_WhenPrinterIdIsNullOrEmpty()
    {
        var opcuaService = _mocker.CreateInstance<ResmarkOPCUAService>();

        var request = new OPCUARequest { PrinterId = string.Empty };

        await Assert.ThrowsAsync<PrinterNotFoundException>(async () => await opcuaService.CallMethodAsync(request, default));
    }

    [Fact]
    public void ValidateAndReturnResult_ValidCallResponse_ReturnsResult()
    {
        StatusCode expectedGoodCode = StatusCodes.Good;

        var callResponse = new CallResponse
        {
            ResponseHeader = new ResponseHeader { ServiceResult = expectedGoodCode },
            Results = new[] { new CallMethodResult { StatusCode = expectedGoodCode } }
        };

        var result = ResmarkOPCUAService.ValidateAndReturnResult(callResponse);

        Assert.Equal(expectedGoodCode, result.StatusCode);
    }

    [Fact]
    public void ValidateAndReturnResult_ThrowsOPCUACommunicationFailedException_WhenResponseHeaderIsNull()
    {
        StatusCode expectedGoodCode = StatusCodes.Good;

        var callResponse = new CallResponse
        {
            ResponseHeader = null,
            Results = new[] { new CallMethodResult { StatusCode = expectedGoodCode } }
        };

        Assert.Throws<OPCUACommunicationFailedException>(() => ResmarkOPCUAService.ValidateAndReturnResult(callResponse));
    }

    [Fact]
    public void ValidateAndReturnResult_ThrowsOPCUACommunicationFailedException_WhenResultsIsNull()
    {
        StatusCode expectedGoodCode = StatusCodes.Good;

        var callResponse = new CallResponse
        {
            ResponseHeader = new ResponseHeader { ServiceResult = expectedGoodCode },
            Results = null
        };

        Assert.Throws<OPCUACommunicationFailedException>(() => ResmarkOPCUAService.ValidateAndReturnResult(callResponse));
    }

    [Theory]
    [InlineData(StatusCodes.BadSessionIdInvalid)]
    [InlineData(StatusCodes.BadTooManyPublishRequests)]
    [InlineData(StatusCodes.BadTcpEndpointUrlInvalid)]
    [InlineData(StatusCodes.BadUnexpectedError)]
    [InlineData(StatusCodes.BadWaitingForInitialData)]
    [InlineData(StatusCodes.BadConfigurationError)]
    [InlineData(StatusCodes.BadConnectionClosed)]
    [InlineData(StatusCodes.BadInvalidState)]
    [InlineData(StatusCodes.BadNoCommunication)]
    // This needs to be uint because these are not real enum values
    public void ValidateAndReturnResult_ThrowsOPCUACommunicationFailedException_WhenServiceResultIsBad(uint badStatusCode)
    {
        var callResponse = new CallResponse
        {
            ResponseHeader = new ResponseHeader { ServiceResult = badStatusCode },
            Results = new[] { new CallMethodResult { StatusCode = StatusCodes.Good } }
        };

        Assert.Throws<OPCUACommunicationFailedException>(() => ResmarkOPCUAService.ValidateAndReturnResult(callResponse));
    }

    [Theory]
    [InlineData(StatusCodes.BadSessionIdInvalid)]
    [InlineData(StatusCodes.BadTooManyPublishRequests)]
    [InlineData(StatusCodes.BadTcpEndpointUrlInvalid)]
    [InlineData(StatusCodes.BadUnexpectedError)]
    [InlineData(StatusCodes.BadWaitingForInitialData)]
    [InlineData(StatusCodes.BadConfigurationError)]
    [InlineData(StatusCodes.BadConnectionClosed)]
    [InlineData(StatusCodes.BadInvalidState)]
    [InlineData(StatusCodes.BadNoCommunication)]
    // This needs to be uint because these are not real enum values
    public void ValidateAndReturnResult_ThrowsOPCUACommunicationFailedException_WhenStatusCodeIsBad(uint badStatusCode)
    {
        var callResponse = new CallResponse
        {
            ResponseHeader = new ResponseHeader { ServiceResult = StatusCodes.Good },
            Results = new[] { new CallMethodResult { StatusCode = badStatusCode } }
        };

        Assert.Throws<OPCUACommunicationFailedException>(() => ResmarkOPCUAService.ValidateAndReturnResult(callResponse));
    }

    [Theory]
    [InlineData(OPCUAMethods.GetStoredMessageList)]
    [InlineData(OPCUAMethods.ResumePrinting)]
    [InlineData(OPCUAMethods.StopPrinting)]
    [InlineData(OPCUAMethods.PrintStoredMessage)]
    [InlineData(OPCUAMethods.GetMessageVariableData)]
    [InlineData(OPCUAMethods.SetMessageVariableData)]
    public void GenerateCallRequest_ReturnsCallRequest_WithCorrectMethodId(OPCUAMethods method)
    {
        var inputArgs = new[] { new(1), new Variant("test") };

        var callRequest = ResmarkOPCUAService.GenerateCallRequest(method, inputArgs);

        Assert.Equal($"ns=2;s={method}", callRequest.MethodsToCall?[0]?.MethodId?.ToString());
    }

    [Fact]
    public void GenerateCallRequest_ReturnsCallRequest_WithCorrectObjectId()
    {
        const OPCUAMethods method = OPCUAMethods.GetMessageVariableData;
        var inputArgs = new[] { new(1), new Variant("test") };

        var callRequest = ResmarkOPCUAService.GenerateCallRequest(method, inputArgs);

        Assert.Equal(ObjectIds.ObjectsFolder, callRequest.MethodsToCall?[0]?.ObjectId?.ToString());
    }

    [Fact]
    public void GenerateCallRequest_ReturnsCallRequest_WithCorrectInputArguments()
    {
        const OPCUAMethods method = OPCUAMethods.GetMessageVariableData;
        var inputArgs = new[] { new(1), new Variant("test") };

        var callRequest = ResmarkOPCUAService.GenerateCallRequest(method, inputArgs);

        Assert.Equal(inputArgs, callRequest.MethodsToCall?[0]?.InputArguments);
    }
}