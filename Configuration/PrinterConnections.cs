namespace BoxwriterResmarkInterop.Configuration;

using System.Text.Json;

public class PrinterConnections
{
    public List<PrinterConnectionInfo> Printers { get; set; } = new List<PrinterConnectionInfo>();

    public async Task SaveSettings()
    {
        var filepath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        var settings = JsonSerializer.Serialize<PrinterConnections>(this, new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(filepath, settings);
    }
}