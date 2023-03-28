﻿namespace BoxwriterResmarkInterop.OPCUA;

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

    public async Task<CallMethodResult> CallMethodAsync(OPCUARequest request, CancellationToken stoppingToken) =>
        await CallMethodAsync(request.PrinterId, request.Method, request.GetArgsAsVariant(), stoppingToken);

    private async Task<CallMethodResult> CallMethodAsync(
        string printerId,
        OPCUAMethods method,
        Variant[] inputArgs,
        CancellationToken stoppingToken)
    {
        if (printerId is null)
        {
            throw new PrinterNotFoundException("Printer Id was null");
        }

        var channel = await OpenChannel(printerId, stoppingToken);

        var response = await MakeCallRequest(method, channel, inputArgs, stoppingToken);

        var results = response.Results ??
                      throw new OPCUACommunicationFailedException("Results of the call was null");

        var callMethodResult = results.First();

        if (callMethodResult is null || !StatusCode.IsGood(callMethodResult.StatusCode))
        {
            throw new OPCUACommunicationFailedException(
                $"{method} OPCUA call failed to get a valid response: {GetStatusCodeMessage()} {callMethodResult?.StatusCode}");
        }

        return callMethodResult;

        string GetStatusCodeMessage() => StatusCodes.GetDefaultMessage(callMethodResult?.StatusCode ?? StatusCodes.BadRequestNotComplete);
    }

    private async Task<CallResponse> MakeCallRequest(
        OPCUAMethods method,
        UaTcpSessionChannel channel,
        Variant[] inputArgs,
        CancellationToken stoppingToken)
    {
        var request = new CallMethodRequest
        {
            MethodId = NodeId.Parse($"ns=2;s={method}"),
            ObjectId = NodeId.Parse(ObjectIds.ObjectsFolder),
            InputArguments = inputArgs
        };

        var callRequest = new CallRequest { MethodsToCall = new[] { request } };

        _logger.LogInformation("Making {Method} OPCUA call", method);

        var response = await channel.CallAsync(callRequest, stoppingToken);

        var serviceResult = response.ResponseHeader?.ServiceResult ?? StatusCodes.BadCommunicationError;

        if (!StatusCode.IsGood(serviceResult))
        {
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

    private UaTcpSessionChannel GetSessionChannel(string connectionName)
    {
        if (!_channelLookup.ContainsKey(connectionName))
        {
            _channelLookup[connectionName] = new UaTcpSessionChannel(_applicationDescription, null,
                new AnonymousIdentity(), GetAddressFromCache(connectionName), SecurityPolicyUris.None);
        }

        return _channelLookup[connectionName];
    }
}