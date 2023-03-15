namespace BoxwriterResmarkInterop.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class CommandNameAttribute : Attribute
    {
        public string CommandName { get; }

        public CommandNameAttribute(string commandName)
        {
            CommandName = commandName;
        }
    }
}