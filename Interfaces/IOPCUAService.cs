namespace BoxwriterResmarkInterop.Interfaces;

using Workstation.ServiceModel.Ua;

public interface IOPCUAService
{
    Task<CallMethodResult?[]?> CallMethodAsync(string connectionName, string method, CancellationToken stoppingToken);
}