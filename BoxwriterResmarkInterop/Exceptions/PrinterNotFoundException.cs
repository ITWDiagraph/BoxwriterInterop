namespace BoxwriterResmarkInterop.Exceptions;

public class PrinterNotFoundException : Exception
{
    public PrinterNotFoundException() : base()
    {
    }

    public PrinterNotFoundException(string message) : base(message)
    {
    }

    public PrinterNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}