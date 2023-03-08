namespace BoxwriterResmarkInterop.OPCUA;

using System.Net;
using System.Threading.Channels;

using Interfaces;

using Workstation.ServiceModel.Ua;
using Workstation.ServiceModel.Ua.Channels;

public class ResmarkOPCUAService : IOPCUAService
{
    private const string ApplicationName = "BoxwriterResmarkInterop";
    private readonly ILogger<ResmarkOPCUAService> _logger;

    private readonly Dictionary<string, UaTcpSessionChannel> ChannelLookup =
        new Dictionary<string, UaTcpSessionChannel>();

    public ResmarkOPCUAService(ILogger<ResmarkOPCUAService> logger)
    {
        _logger = logger;
    }

    private ApplicationDescription _applicationDescription =>
        new ApplicationDescription
        {
            ApplicationName = ApplicationName,
            ApplicationType = ApplicationType.Client,
            ApplicationUri = $"urn:{Dns.GetHostName()}:{ApplicationName}"
        };

    public async Task<CallMethodResult?[]?> CallMethodAsync(
        string connectionName,
        string method,
        CancellationToken stoppingToken)
    {
        var channel = GetSessionChannel(connectionName);

        if (channel.State != CommunicationState.Opened)
        {
            try
            {
                _logger.LogInformation("Attempting to start OPCUA connection..");
                await channel.OpenAsync(stoppingToken);
            }
            catch (Exception e)
            {
                _logger.LogError("Connection failure", e);
            }
        }

        if (channel.State != CommunicationState.Opened)
        {
            _logger.LogError("Channel could not be opened");

            throw new ChannelClosedException();
        }

        var request = new CallMethodRequest
        {
            MethodId = NodeId.Parse($"ns=2;s={method}"),
            ObjectId = NodeId.Parse(ObjectIds.ObjectsFolder),
            InputArguments = Array.Empty<Variant>()
        };

        var callRequest = new CallRequest { MethodsToCall = new[] { request } };

        var response = await channel.CallAsync(callRequest, stoppingToken);

        var serviceResult = response.ResponseHeader?.ServiceResult;

        if (!serviceResult.HasValue || !StatusCode.IsGood(serviceResult.Value))
        {
            _logger.LogError("OPCUA call failed");

            throw new Exception("OPCUA call failed");
        }

        return response.Results;
    }

    //TODO: don't hard code this
    private string GetAddress(string connectionName)
    {
        return "opc.tcp://172.16.11.236:16664";
    }

    private UaTcpSessionChannel GetSessionChannel(string connectionName)
    {
        if (!ChannelLookup.ContainsKey(connectionName))
        {
            ChannelLookup[connectionName] = new UaTcpSessionChannel(_applicationDescription, null,
                new AnonymousIdentity(), GetAddress(connectionName), SecurityPolicyUris.None);
        }

        return ChannelLookup[connectionName];
    }
}