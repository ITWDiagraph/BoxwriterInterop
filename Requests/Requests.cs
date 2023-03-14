namespace BoxwriterResmarkInterop.Requests;

using MediatR;

public sealed record TCPRequest(string Data) : IRequest<StringResponse>;

public sealed record GetTasksRequest(string Data) : IRequest<StringResponse>;

public sealed record StartTaskRequest(string Data) : IRequest<StringResponse>;