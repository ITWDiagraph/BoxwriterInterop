namespace BoxwriterResmarkInterop.Configuration;

using System.Text.Json;

using static Constants;

public class PrinterConnections
{
    public List<PrinterConnectionInfo> Printers { get; set; } = new List<PrinterConnectionInfo>();

    public async Task SaveSettings(IHostEnvironment environment)
    {
        var filepath = Path.Combine(AppContext.BaseDirectory, $"{AppSettings}.{environment.EnvironmentName}.json");

        var settings = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(filepath, settings);
    }
}