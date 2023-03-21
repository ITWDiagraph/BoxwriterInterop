namespace BoxwriterResmarkInterop.Handlers;

using Configuration;

using Extensions;

using MediatR;

using Microsoft.Extensions.Options;

using Requests;

using static Constants;

public class AddLineCommandHandler : IRequestHandler<AddLineRequest, StringResponse>
{
    private readonly PrinterConnections _printerConnections;

    public AddLineCommandHandler(IOptions<PrinterConnections> configuration)
    {
        _printerConnections = configuration.Value;
    }

    public async Task<StringResponse> Handle(AddLineRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();
        var ipAddress = request.Data.ExtractAdditionalParameter();

        return await AddLineAndSaveSettings(printerId, ipAddress);
    }

    private async Task<StringResponse> AddLineAndSaveSettings(string printerId, string ipAddress)
    {
        var newConnection = new PrinterConnectionInfo
        {
            IpAddress = ipAddress,
            PrinterId = printerId
        };

        if (_printerConnections.Printers.Any(p => p.PrinterId == printerId))
        {
            _printerConnections.Printers.Remove(_printerConnections.Printers.First(p => p.PrinterId == printerId));
        }

        _printerConnections.Printers.Add(newConnection);

        await _printerConnections.SaveSettings();

        return new StringResponse(AddLine, printerId, true);
    }
}