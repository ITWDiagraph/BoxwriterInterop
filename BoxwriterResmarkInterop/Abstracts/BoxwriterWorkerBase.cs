
using System.Net;

namespace BoxwriterResmarkInterop.Abstracts;
public abstract class BoxwriterWorkerBase : BackgroundService
{
    ///<inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = new List<Task>();

        foreach (var ipAddress in await Dns.GetHostAddressesAsync(Dns.GetHostName(), stoppingToken))
        {
            tasks.Add(ListenAsync(ipAddress, stoppingToken));
        }

        tasks.Add(ListenAsync(IPAddress.Loopback, stoppingToken));

        await Task.WhenAll(tasks);
    }

    protected abstract Task ListenAsync(IPAddress address, CancellationToken stoppingToken);
}