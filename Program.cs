using BoxwriterResmarkInterop;
using BoxwriterResmarkInterop.Configuration;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.TCP;
using BoxwriterResmarkInterop.UDP;

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, builder) =>
        builder
            .AddJsonFile($"{nameof(PrinterConnections)}.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json"))
    .ConfigureLogging(logger => logger.AddConsole())
    .ConfigureServices((builder, services) =>
    {
        services.Configure<PrinterConnections>(connections => connections.Printers = builder.Configuration
                .GetSection("PrinterConnections")
                .Get<List<PrinterConnectionInfo>>() ?? throw new InvalidOperationException());

        services.AddSingleton<IUdpDataHandler, BoxwriterUDPHandler>()
                .AddSingleton<IOPCUAService, ResmarkOPCUAService>()
                .AddHostedService<BoxwriterUDPWorker>()
                .AddHostedService<BoxwriterTCPWorker>()
                .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
    })
    .Build()
    .Run();
