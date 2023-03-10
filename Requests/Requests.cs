namespace BoxwriterResmarkInterop.Requests;

using Mediator;

public sealed record StringResponse(string data);

public sealed record TCPRequest(string data) : IRequest<StringResponse>;

public sealed record GetTaskRequest(string data) : IRequest<StringResponse>;