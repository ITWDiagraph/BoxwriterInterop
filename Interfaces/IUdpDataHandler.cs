namespace BoxwriterResmarkInterop.Interfaces;

using System.Net;
using System.Net.Sockets;

public interface IUdpDataHandler
{
    void ProcessData(UdpReceiveResult data, IPAddress ipAddress);
}