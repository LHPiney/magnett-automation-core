using System.Threading;

namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public interface INodeBase  
{
    public CommonNamedKey Key { get; }
    public string Name => Key?.Name;
    public NodeState State { get; }
    public bool IsInitialized { get; }
    public Task Init(CancellationToken cancellationToken = default);
}    