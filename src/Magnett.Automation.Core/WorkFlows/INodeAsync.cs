using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.WorkFlows
{
    public interface INodeAsync : INodeBase
    {
        Task<NodeExit> Execute(Context context);
    }
}