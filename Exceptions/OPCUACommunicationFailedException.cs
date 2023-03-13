namespace BoxwriterResmarkInterop.Exceptions;

public class OPCUACommunicationFailedException : Exception
{
    public OPCUACommunicationFailedException() : base()
    {
    }

    public OPCUACommunicationFailedException(string message) : base(message)
    {
    }

    public OPCUACommunicationFailedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}