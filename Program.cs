using BoxwriterResmarkInterop;
using BoxwriterResmarkInterop.Interfaces;
using BoxwriterResmarkInterop.OPCUA;
using BoxwriterResmarkInterop.Services;
using BoxwriterResmarkInterop.TCP;
using BoxwriterResmarkInterop.UDP;

Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logger => logger.AddConsole())
    .ConfigureServices(services =>
    {
        services.AddSingleton<IUdpDataHandler, BoxwriterUDPHandler>();
        services.AddSingleton<IOPCUAService, ResmarkOPCUAService>();
        services.AddSingleton<ICommandNameRegistrationService, CommandNameRegistrationService>();
        services.AddHostedService<BoxwriterUDPWorker>();
        services.AddHostedService<BoxwriterTCPWorker>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
    })
    .Build()
    .Run();