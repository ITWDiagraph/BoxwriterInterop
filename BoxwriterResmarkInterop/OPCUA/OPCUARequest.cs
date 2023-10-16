
using Workstation.ServiceModel.Ua;

namespace BoxwriterResmarkInterop.OPCUA;
public class OPCUARequest
{
    public string PrinterId { get; init; } = string.Empty;
    public OPCUAMethods Method { get; init; }
    public int? TaskNumber { get; init; }
    public object[]? InputArgs { get; init; }

    public Variant[] GetArgsAsVariant()
    {
        var args = new List<Variant>();

        if (TaskNumber.HasValue)
        {
            args.Add(new Variant(TaskNumber.Value));
        }

        if (InputArgs != null)
        {
            args.AddRange(InputArgs.Select(arg => new Variant(arg)));
        }

        return args.ToArray();
    }
}