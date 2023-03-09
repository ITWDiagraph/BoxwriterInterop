using BoxwriterResmarkInterop;
using BoxwriterResmarkInterop.Configuration;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.TCP;
using BoxwriterResmarkInterop.UDP;

var PrinterConnections = new List<PrinterConnection>();

Host.CreateDefaultBuilder(args).ConfigureLogging(logger => { logger.AddConsole(); }).ConfigureServices(
    (builder, services) =>
    {
        builder.Configuration.GetSection(nameof(PrinterConnections)).Bind(PrinterConnections);

        services.AddSingleton<IUdpDataHandler, BoxwriterUDPHandler>();

        services.AddSingleton<IOPCUAService, ResmarkOPCUAService>(provider =>
            new ResmarkOPCUAService(PrinterConnections, provider.GetService<ILogger<ResmarkOPCUAService>>()));

        services.AddHostedService<BoxwriterUDPWorker>();
        services.AddHostedService<BoxwriterTCPWorker>();
        services.AddMediator();
    }).Build().Run();