using BoxwriterResmarkInterop.Configuration;
using BoxwriterResmarkInterop.Requests;

using MediatR;

using Microsoft.Extensions.Options;

namespace BoxwriterResmarkInterop.Handlers;
public class GetLinesRequestHandler : IRequestHandler<GetLinesRequest, StringResponse>
{
    private readonly PrinterConnections _printerConnections;

    public GetLinesRequestHandler(IOptions<PrinterConnections> configuration) => _printerConnections = configuration.Value;

    public Task<StringResponse> Handle(GetLinesRequest request, CancellationToken cancellationToken) =>
        Task.FromResult(new StringResponse(Constants.GetLines, GetPrinters()));

    private IEnumerable<string> GetPrinters() => _printerConnections.Printers.Select(printer => printer.PrinterId);
}