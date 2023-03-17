namespace BoxwriterResmarkInterop.Configuration;

using System.Text.Json;

public class PrinterConnections
{
    public List<PrinterConnectionInfo> Printers { get; set; } = new List<PrinterConnectionInfo>();

    public async Task SaveSettings(IHostEnvironment environment)
    {
        var filepath = Path.Combine(AppContext.BaseDirectory, $"appsettings.{environment.EnvironmentName}.json");

        var settings = JsonSerializer.Serialize<PrinterConnections>(this, new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(filepath, settings);
    }
}