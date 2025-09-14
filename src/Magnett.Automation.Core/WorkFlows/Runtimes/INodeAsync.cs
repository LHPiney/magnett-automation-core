using System.Threading;

namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public interface INodeAsync : INodeBase
{
    public Task<NodeExit> ExecuteAsync(Context context, CancellationToken cancellationToken);
}