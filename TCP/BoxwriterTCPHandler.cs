﻿namespace BoxwriterResmarkInterop.TCP;

using MediatR;

using Requests;

public class BoxwriterTCPHandler : IRequestHandler<TCPRequest, StringResponse>
{
    private readonly IMediator _mediator;

    public BoxwriterTCPHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<StringResponse> Handle(TCPRequest request, CancellationToken cancellationToken)
    {
        object tcpRequest = request.data switch
        {
            var x when x.Contains("Get tasks") => new GetTasksRequest(x),
            var y when y.Contains("Start task") => new StartTaskRequest(y),
            _ => throw new ArgumentOutOfRangeException()
        };

        return await _mediator.Send(tcpRequest, cancellationToken).ConfigureAwait(false) as StringResponse;
    }
}