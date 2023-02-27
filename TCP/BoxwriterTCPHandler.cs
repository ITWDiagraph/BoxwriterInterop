namespace BoxwriterResmarkInterop.TCP;

using System.Net.Sockets;
using System.Text;

using Interfaces;

public class BoxwriterTCPHandler : ITCPDataHandler
{
    public async Task ProcessData(string data, NetworkStream stream, CancellationToken cancellationToken = default)
    {
        var response = Encoding.ASCII.GetBytes(data + " From server");

        await stream.WriteAsync(response, 0, response.Length, cancellationToken);

        await stream.FlushAsync(cancellationToken);
    }
}