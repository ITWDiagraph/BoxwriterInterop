namespace BoxwriterResmarkInterop.Interfaces;

using System.Net;
using System.Net.Sockets;

public interface IUdpDataHandler
{
    Task ProcessDataAsync(UdpReceiveResult data, IPAddress ipAddress);
}