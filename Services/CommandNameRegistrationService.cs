namespace BoxwriterResmarkInterop.Services;

using Interfaces;

using System.Reflection;

using Attributes;

public class CommandNameRegistrationService : ICommandNameRegistrationService
{
    private readonly ILogger<CommandNameRegistrationService> _logger;
    private readonly Dictionary<string, Type> _commandNameRegistry = new Dictionary<string, Type>();

    public CommandNameRegistrationService(ILogger<CommandNameRegistrationService> logger)
    {
        _logger = logger;

        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(IsCommandNameRegistered))
        {
            var commandName = GetCommandNameAttribute(type).CommandName;

            if (_commandNameRegistry.ContainsKey(commandName))
            {
                _logger.LogError("{CommandName} has already been registered to {CommandType}", commandName, _commandNameRegistry[commandName].Name);

                throw new AmbiguousMatchException(
                    $"{commandName} has already been registered to {_commandNameRegistry[commandName]}");
            }

            _commandNameRegistry[commandName] = type;

            _logger.LogInformation("Registered command \"{CommandName}\" to type {CommandType}", commandName, type.Name);
        }
    }

    private static bool IsCommandNameRegistered(Type type)
    {
        return GetCommandNameAttribute(type) is not null;
    }

    private static CommandNameAttribute GetCommandNameAttribute(Type type) => type.GetCustomAttribute<CommandNameAttribute>();

    public IReadOnlyDictionary<string, Type> CommandNameRegistry => _commandNameRegistry;
}