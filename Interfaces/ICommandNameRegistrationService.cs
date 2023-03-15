namespace BoxwriterResmarkInterop.Interfaces;

public interface ICommandNameRegistrationService
{
    IReadOnlyDictionary<string, Type> CommandNameRegistry { get; }
}