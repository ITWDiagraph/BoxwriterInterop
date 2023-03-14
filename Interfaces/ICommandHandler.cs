using Workstation.ServiceModel.Ua;

namespace BoxwriterResmarkInterop.Interfaces;

public interface ICommandHandler
{
    public string CommandName { get; }

    public string GetResponseData(CallMethodResult result);
}