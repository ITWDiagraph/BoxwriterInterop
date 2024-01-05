
using System.Net;
using System.Net.Sockets;

namespace BoxwriterResmarkInterop.Interfaces;
public interface IUdpDataHandler
{
    Task ProcessDataAsync(UdpReceiveResult data, IPAddress ipAddress);
}