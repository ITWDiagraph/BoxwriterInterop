using System.Text.Json;

namespace BoxwriterResmarkInterop.Configuration;
public class PrinterConnections
{
    public List<PrinterConnectionInfo> Printers { get; set; } = new List<PrinterConnectionInfo>();

    public async Task SaveSettings()
    {
        var filepath = Path.Combine(AppContext.BaseDirectory, $"{nameof(PrinterConnections)}.json");

        var settings = JsonSerializer
            .Serialize(this, new JsonSerializerOptions { WriteIndented = true });

        await File.WriteAllTextAsync(filepath, settings);
    }
}