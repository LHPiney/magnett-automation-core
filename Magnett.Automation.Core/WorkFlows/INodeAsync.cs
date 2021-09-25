using System.Threading.Tasks;

namespace Magnett.Automation.Core.WorkFlows
{
    public interface INodeAsync
    {
        Task Execute();
    }
}