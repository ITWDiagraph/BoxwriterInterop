using System.Net;
using System.Net.Sockets;
using System.Text;

using BoxwriterResmarkInterop.Interfaces;

namespace BoxwriterResmarkInterop;
public class BoxwriterUDPHandler : IUdpDataHandler
{
    private readonly ILogger<BoxwriterUDPHandler> _logger;

    public BoxwriterUDPHandler(ILogger<BoxwriterUDPHandler> logger) => _logger = logger;

    public async Task ProcessDataAsync(UdpReceiveResult data, IPAddress ipAddress)
    {
        _logger.LogInformation("Udp Handler received data packet {Packet}", data);

        var asciiData = Encoding.ASCII.GetString(data.Buffer);

        if (asciiData.Contains("Locate BoxWriter"))
        {
            _logger.LogInformation("Received locator packet sending response");
            var response = Encoding.ASCII.GetBytes($"{{Locate BoxWriter, {ipAddress}, 2202, {Dns.GetHostName()}}}");
            var client = new UdpClient();
            client.Connect(data.RemoteEndPoint);
            await client.SendAsync(response);
        }
    }
}