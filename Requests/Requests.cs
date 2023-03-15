namespace BoxwriterResmarkInterop.Requests;

using Attributes;

using MediatR;

[CommandName(Constants.GetTasks)]
public sealed record GetTasksRequest(string Data) : IRequest<StringResponse>;

[CommandName(Constants.StartTask)]
public sealed record StartTaskRequest(string Data) : IRequest<StringResponse>;