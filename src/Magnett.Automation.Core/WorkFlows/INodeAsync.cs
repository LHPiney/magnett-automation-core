using System.Threading.Tasks;

namespace Magnett.Automation.Core.WorkFlows
{
    public interface INodeAsync : INodeBase
    {
        Task<NodeExit> Execute();
    }
}