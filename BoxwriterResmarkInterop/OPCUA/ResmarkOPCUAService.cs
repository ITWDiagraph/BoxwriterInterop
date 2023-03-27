namespace BoxwriterResmarkInterop.OPCUA;

using System.Net;

using Configuration;

using Exceptions;

using Interfaces;

using Microsoft.Extensions.Options;

using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

public class ResmarkOPCUAService : IOPCUAService
{
    private const string ApplicationName = "BoxwriterResmarkInterop";
    private readonly ApplicationDescription _applicationDescription;

    private readonly Dictionary<string, UaTcpSessionChannel> _channelLookup = new();
    private readonly ILogger<ResmarkOPCUAService> _logger;

    private readonly PrinterConnections _printerConnections;

    public ResmarkOPCUAService(IOptions<PrinterConnections> configuration, ILogger<ResmarkOPCUAService> logger)
    {
        _logger = logger;

        _applicationDescription = new ApplicationDescription
        {
            ApplicationName = ApplicationName,
            ApplicationType = ApplicationType.Client,
            ApplicationUri = $"urn:{Dns.GetHostName()}:{ApplicationName}"
        };

        _printerConnections = configuration.Value;
    }

    public async Task<CallMethodResult> CallMethodAsync(OPCUARequest request, CancellationToken stoppingToken)
    {
        if (string.IsNullOrWhiteSpace(request.PrinterId))
        {
            throw new PrinterNotFoundException("Printer Id was empty");
        }

        var channel = await OpenChannel(request.PrinterId, stoppingToken);

        var inputArgs = request.GetArgsAsVariant();

        var callRequest = GenerateCallRequest(request.Method, inputArgs);

        _logger.LogInformation("Making {Method} OPCUA call{OptionalArgs}", request.Method,
            inputArgs.Any() ? $" with args {string.Join(",", inputArgs)}" : string.Empty);

        var callResponse = await MakeOPCUACall(channel, callRequest, stoppingToken);

        try
        {
            return ValidateAndReturnResult(callResponse);
        }
        catch (OPCUACommunicationFailedException e)
        {
            _logger.LogError("OPCUA call {Method} failed with reason {Exception}", request.Method, e);

            throw;
        }
    }

    public static CallMethodResult ValidateAndReturnResult(CallResponse? callResponse)
    {
        var serviceResult = callResponse?.ResponseHeader?.ServiceResult ?? StatusCodes.BadCommunicationError;

        if (!StatusCode.IsGood(serviceResult))
        {
            throw new OPCUACommunicationFailedException("OPCUA call failed");
        }

        var result = callResponse?.Results?.FirstOrDefault() ??
                     throw new OPCUACommunicationFailedException("Results of call was null");

        if (!StatusCode.IsGood(result.StatusCode))
        {
            throw new OPCUACommunicationFailedException(
                $"OPCUA call failed to get a valid response: {StatusCodes.GetDefaultMessage(result.StatusCode)} {result.StatusCode}");
        }

        return result;
    }

    public static CallRequest GenerateCallRequest(OPCUAMethods method, Variant[] inputArgs)
    {
        var request = new CallMethodRequest
        {
            MethodId = NodeId.Parse($"ns=2;s={method}"),
            ObjectId = NodeId.Parse(ObjectIds.ObjectsFolder),
            InputArguments = inputArgs
        };

        return new CallRequest { MethodsToCall = new[] { request } };
    }

    public static async Task<CallResponse> MakeOPCUACall(
        UaTcpSessionChannel channel,
        CallRequest callRequest,
        CancellationToken stoppingToken) =>
        await channel.CallAsync(callRequest, stoppingToken);

    private async Task<UaTcpSessionChannel> OpenChannel(string printerId, CancellationToken stoppingToken)
    {
        var channel = await GetSessionChannel(printerId);

        if (channel.State != CommunicationState.Opened)
        {
            _logger.LogInformation("Attempting to start OPCUA connection..");

            try
            {
                await channel.OpenAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                throw new OPCUACommunicationFailedException($"Channel could not be opened: {ex}");
            }
        }

        _logger.LogInformation("OPCUA connection successful");

        return channel;
    }

    private string GetAddressFromCache(string printerId)
    {
        var printer = _printerConnections.Printers.FirstOrDefault(p => p.PrinterId == printerId) ??
                      throw new PrinterNotFoundException($"Printer Id {printerId} was not found");

        return $"opc.tcp://{printer.IpAddress}:16664";
    }

    private async Task<UaTcpSessionChannel> GetSessionChannel(string connectionName)
    {
        if (_channelLookup.ContainsKey(connectionName))
        {
            if (_channelLookup[connectionName].State == CommunicationState.Opened)
            {
                _logger.LogDebug("Channel for {Connection} exists and is open", connectionName);

                return _channelLookup[connectionName];
            }

            _logger.LogDebug("Channel for {Connection} is not open, state: {State}", connectionName, _channelLookup[connectionName].State);

            try
            {
                await _channelLookup[connectionName].AbortAsync();
            }
            catch (Exception e)
            {
                _logger.LogDebug("Error while trying to close bad channel: {Exception}", e);
            }
        }

        _logger.LogDebug("Creating a new channel for {Connection}", connectionName);

        _channelLookup[connectionName] = new UaTcpSessionChannel(_applicationDescription, null,
            new AnonymousIdentity(), GetAddressFromCache(connectionName), SecurityPolicyUris.None);

        return _channelLookup[connectionName];
    }
}