namespace BoxwriterResmarkInterop.Interfaces;

using BoxwriterResmarkInterop.OPCUA;
using Workstation.ServiceModel.Ua;

public interface IOPCUAService
{
    Task<CallMethodResult> CallMethodAsync(OPCUARequest request, CancellationToken stoppingToken);
}