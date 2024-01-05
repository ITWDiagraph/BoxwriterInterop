using BoxwriterResmarkInterop.Configuration;
using BoxwriterResmarkInterop.Extensions;
using BoxwriterResmarkInterop.Requests;

using MediatR;

using Microsoft.Extensions.Options;

namespace BoxwriterResmarkInterop.Handlers;
public class AddLineCommandHandler : IRequestHandler<AddLineRequest, StringResponse>
{
    private readonly PrinterConnections _printerConnections;

    public AddLineCommandHandler(IOptions<PrinterConnections> configuration) => _printerConnections = configuration.Value;

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

        _printerConnections.Printers.RemoveAll(p => p.PrinterId == printerId);

        _printerConnections.Printers.Add(newConnection);

        await _printerConnections.SaveSettings();

        return new StringResponse(Constants.AddLine, printerId, true);
    }
}