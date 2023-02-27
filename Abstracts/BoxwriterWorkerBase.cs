namespace BoxwriterResmarkInterop.Abstracts;

using System.Net;

public abstract class BoxwriterWorkerBase : BackgroundService
{
    /// <summary>
    ///     This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The
    ///     implementation should return a task that represents
    ///     the lifetime of the long running operation(s) being performed.
    /// </summary>
    /// <param name="stoppingToken">
    ///     Triggered when
    ///     <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is
    ///     called.
    /// </param>
    /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the long running operations.</returns>
    /// <remarks>
    ///     See <see href="https://docs.microsoft.com/dotnet/core/extensions/workers">Worker Services in .NET</see> for
    ///     implementation guidelines.
    /// </remarks>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = new List<Task>();
        foreach (var ipAddress in await Dns.GetHostAddressesAsync(Dns.GetHostName(),stoppingToken))
        {
            tasks.Add(StartListeningAsync(ipAddress, stoppingToken));
        }

        await Task.WhenAll(tasks);
    }

    protected abstract Task StartListeningAsync(IPAddress address, CancellationToken stoppingToken);
}