
using BoxwriterResmarkInterop.OPCUA;

using Workstation.ServiceModel.Ua;

namespace BoxwriterResmarkInterop.Interfaces;
public interface IOPCUAService
{
    Task<CallMethodResult> CallMethodAsync(OPCUARequest request, CancellationToken stoppingToken);
}