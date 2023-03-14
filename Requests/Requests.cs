namespace BoxwriterResmarkInterop.Requests;

using MediatR;

public sealed record TCPRequest(string data) : IRequest<StringResponse>;

public sealed record GetTasksRequest(string data) : IRequest<StringResponse>;

public sealed record StartTaskRequest(string data) : IRequest<StringResponse>;