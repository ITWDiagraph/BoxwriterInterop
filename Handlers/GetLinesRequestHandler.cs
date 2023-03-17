﻿namespace BoxwriterResmarkInterop.Handlers;

using Configuration;

using MediatR;

using Microsoft.Extensions.Options;

using Requests;

using static Constants;

public class GetLinesRequestHandler : IRequestHandler<GetLinesRequest, StringResponse>
{
    private readonly PrinterConnections _printerConnections;

    public GetLinesRequestHandler(IOptions<PrinterConnections> configuration)
    {
        _printerConnections = configuration.Value;
    }

    public async Task<StringResponse> Handle(GetLinesRequest request, CancellationToken cancellationToken)
    {
        return new StringResponse(GetLines, GetResponseData());
    }

    private IEnumerable<string> GetResponseData()
    {
        return _printerConnections.Printers.Select(printer => printer.PrinterId).ToList();
    }
}