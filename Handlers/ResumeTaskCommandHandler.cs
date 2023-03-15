namespace BoxwriterResmarkInterop.Handlers
{
    using Interfaces;

    public class ResumeTaskCommandHandler : StartTaskCommandHandler
    {
        public ResumeTaskCommandHandler(ILogger<StartTaskCommandHandler> logger, IOPCUAService opcuaService)
            : base(logger, opcuaService) { }
    }
}