namespace BoxwriterResmarkInterop.Interfaces;

using System.Net.Sockets;

public interface ITCPDataHandler
{
    Task ProcessDataAsync(string data, NetworkStream stream, CancellationToken cancellationToken);
}