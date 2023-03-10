namespace BoxwriterResmarkInterop.Requests;

using MediatR;

public sealed record StringResponse(string data);

public sealed record TCPRequest(string data) : IRequest<StringResponse>;

public sealed record GetTaskRequest(string data) : IRequest<StringResponse>;