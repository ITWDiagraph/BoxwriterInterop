using BoxwriterResmarkInterop;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.TCP;
using BoxwriterResmarkInterop.UDP;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logger =>
    {
        logger.AddConsole();
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<IUdpDataHandler, BoxwriterUDPHandler>();
        services.AddSingleton<ITCPDataHandler, BoxwriterTCPHandler>();
        services.AddHostedService<BoxwriterUDPWorker>();
        services.AddHostedService<BoxwriterTCPWorker>();
    })
    .Build();

host.Run();