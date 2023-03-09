﻿namespace BoxwriterResmarkInterop.OPCUA;

using System.Net;
using System.Threading.Channels;

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

    public ResmarkOPCUAService(IEnumerable<PrinterConnection> printerConnections, ILogger<ResmarkOPCUAService> logger)
    {
        _logger = logger;
        _hostName = Dns.GetHostName();

        _applicationDescription = new ApplicationDescription
        {
            ApplicationName = ApplicationName,
            ApplicationType = ApplicationType.Client,
            ApplicationUri = $"urn:{_hostName}:{ApplicationName}"
        };

        _printerConnections = printerConnections;
    }

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

    private string GetAddress(string printerId)
    {
        var printer = _printerConnections.FirstOrDefault(p => p.PrinterId == printerId) ??
                      throw new PrinterNotFoundException();

        return $"opc.tcp://{printer.IpAddress}:16664";
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