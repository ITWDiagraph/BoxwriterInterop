namespace BoxwriterResmarkInterop.TCP;

using Mediator;

public sealed record StringResponse(string data);

public sealed record TCPRequest(string data) : IRequest<StringResponse>;

public class BoxwriterTCPHandler : IRequestHandler<TCPRequest, StringResponse>
{
    private readonly IMediator _mediator;

    public BoxwriterTCPHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async ValueTask<StringResponse> Handle(TCPRequest request, CancellationToken cancellationToken)
    {
        var tcpRequest = request.data switch
        {
            var x when x.Contains("Get tasks") => new GetTaskRequest(x),
            _ => throw new ArgumentOutOfRangeException()
        };

        return await _mediator.Send(tcpRequest, cancellationToken).ConfigureAwait(false);
    }
}

public sealed record GetTaskRequest(string data) : IRequest<StringResponse>;