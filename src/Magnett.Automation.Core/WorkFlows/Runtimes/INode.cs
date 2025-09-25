using System.Threading;

namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public interface INode : INodeBase
{
    public Task<NodeExit> Execute(Context context);
    public Task<NodeExit> Execute(Context context, CancellationToken cancellationToken);
}