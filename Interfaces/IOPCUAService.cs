namespace BoxwriterResmarkInterop.Interfaces;

using Workstation.ServiceModel.Ua;

public interface IOPCUAService
{
    Task<IEnumerable<CallMethodResult>> CallMethodAsync(
        string printerId,
        string method,
        CancellationToken stoppingToken);
}