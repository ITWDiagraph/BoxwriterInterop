namespace BoxwriterResmarkInterop.Requests;

using Attributes;

using MediatR;

[CommandName("Get tasks")]
public sealed record GetTasksRequest(string Data) : IRequest<StringResponse>;

[CommandName("Start task")]
public sealed record StartTaskRequest(string Data) : IRequest<StringResponse>;