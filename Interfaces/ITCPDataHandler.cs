namespace BoxwriterResmarkInterop.Interfaces;

using System.Net.Sockets;

public interface ITCPDataHandler
{
    Task ProcessData(string data, NetworkStream stream, CancellationToken cancellationToken);
}