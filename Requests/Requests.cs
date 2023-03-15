namespace BoxwriterResmarkInterop.Requests;

using MediatR;

public sealed record GetTasksRequest(string Data) : IRequest<StringResponse>;

public sealed record StartTaskRequest(string Data) : IRequest<StringResponse>;

public sealed record LoadTaskRequest(string Data) : IRequest<StringResponse>;

public sealed record ResumeTaskRequest(string Data) : IRequest<StringResponse>;