namespace BoxwriterResmarkInterop.Interfaces;

using Workstation.ServiceModel.Ua;

public interface IOPCUAService
{
    Task<CallMethodResult?> CallMethodAsync(
        string printerId,
        string method,
        CancellationToken stoppingToken,
        params string[] inputArgs);

}