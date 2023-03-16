namespace BoxwriterResmarkInterop.Handlers;

using Configuration;

using Extensions;

using Interfaces;

using MediatR;
using Requests;

using static Constants;

public class AddLineCommandHandler : IRequestHandler<AddLineRequest, StringResponse>
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AddLineCommandHandler> _logger;
    private readonly IOPCUAService _opcuaService;

    public AddLineCommandHandler(ILogger<AddLineCommandHandler> logger, IOPCUAService opcuaService, IConfiguration configuration)
    {
        _logger = logger;
        _opcuaService = opcuaService;
        _configuration = configuration;
    }

    public async Task<StringResponse> Handle(AddLineRequest request, CancellationToken cancellationToken)
    {
        var printerId = request.Data.ExtractPrinterId();
        var ipAddress = request.Data.ExtractAdditionalParameter();
        var result = false;

        var printerConnections = _configuration.GetSection("PrinterConnections").Get<List<PrinterConnection>>();

        if (!printerConnections.Any(p => p.PrinterId == printerId))
        {
            var newConnection = new PrinterConnection()
            {
                IpAddress = ipAddress,
                PrinterId = printerId
            };

            printerConnections.Add(newConnection);
            _configuration.GetSection("PrinterConnections").Bind(printerConnections);

            result = true;
        }

        return new StringResponse(AddLine, printerId, result);
    }
}