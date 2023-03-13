namespace BoxwriterResmarkInterop.OPCUA;

using System.Net;

using Configuration;

using Exceptions;

using Interfaces;

using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

public class ResmarkOPCUAService : IOPCUAService
{
    private const string ApplicationName = "BoxwriterResmarkInterop";
    private readonly ApplicationDescription _applicationDescription;
    private readonly string _hostName;
    private readonly ILogger<ResmarkOPCUAService> _logger;

    private readonly IEnumerable<PrinterConnection> _printerConnections;

    private readonly Dictionary<string, UaTcpSessionChannel> ChannelLookup =
        new Dictionary<string, UaTcpSessionChannel>();

    public ResmarkOPCUAService(IConfiguration configuration, ILogger<ResmarkOPCUAService> logger)
    {
        _logger = logger;
        _hostName = Dns.GetHostName();

        _applicationDescription = new ApplicationDescription
        {
            ApplicationName = ApplicationName,
            ApplicationType = ApplicationType.Client,
            ApplicationUri = $"urn:{_hostName}:{ApplicationName}"
        };

        _printerConnections = new List<PrinterConnection>();
        configuration.GetSection("PrinterConnections").Bind(_printerConnections);
    }

    public async Task<IEnumerable<CallMethodResult?>> CallMethodAsync(
        string printerId,
        string method,
        CancellationToken stoppingToken)
    {
        if (printerId is null)
        {
            _logger.LogError("Printer Id was null");

            throw new PrinterNotFoundException("Printer Id was null");
        }

        var channel = await OpenChannel(printerId, stoppingToken);

        var response = await MakeCallRequest(method, stoppingToken, channel);

        var results = response.Results ?? throw new OPCUACommunicationFailedException("Results of the call was null");

        return results;
    }

    private async Task<CallResponse> MakeCallRequest(
        string method,
        CancellationToken stoppingToken,
        UaTcpSessionChannel channel)
    {
        var request = new CallMethodRequest
        {
            MethodId = NodeId.Parse($"ns=2;s={method}"),
            ObjectId = NodeId.Parse(ObjectIds.ObjectsFolder),
            InputArguments = Array.Empty<Variant>()
        };

        var callRequest = new CallRequest { MethodsToCall = new[] { request } };

        _logger.LogInformation("Making {Method} OPCUA call", method);

        var response = await channel.CallAsync(callRequest, stoppingToken).ConfigureAwait(false);

        var serviceResult = response.ResponseHeader?.ServiceResult;

        if (!serviceResult.HasValue || !StatusCode.IsGood(serviceResult.Value))
        {
            _logger.LogError(" {Method} OPCUA call failed", method);

            throw new OPCUACommunicationFailedException($"{method} OPCUA call failed");
        }

        return response;
    }

    private async Task<UaTcpSessionChannel> OpenChannel(string printerId, CancellationToken stoppingToken)
    {
        var channel = GetSessionChannel(printerId);

        if (channel.State != CommunicationState.Opened)
        {
            _logger.LogInformation("Attempting to start OPCUA connection..");

            try
            {
                await channel.OpenAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError("Channel could not be opened: {ex}", ex);

                throw new OPCUACommunicationFailedException($"Channel could not be opened: {ex}");
            }
        }

        _logger.LogInformation("OPCUA connection successful");

        return channel;
    }

    private string GetAddressFromCache(string printerId)
    {
        var printer = _printerConnections.FirstOrDefault(p => p.PrinterId == printerId) ??
                      throw new PrinterNotFoundException($"Printer Id, {printerId} was not found");

        return $"opc.tcp://{printer.IpAddress}:16664";
    }

    private UaTcpSessionChannel GetSessionChannel(string connectionName)
    {
        if (!ChannelLookup.ContainsKey(connectionName))
        {
            ChannelLookup[connectionName] = new UaTcpSessionChannel(_applicationDescription, null,
                new AnonymousIdentity(), GetAddressFromCache(connectionName), SecurityPolicyUris.None);
        }

        return ChannelLookup[connectionName];
    }
}