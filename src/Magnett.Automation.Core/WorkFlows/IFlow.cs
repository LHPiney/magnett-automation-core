using System;
using System.Threading.Tasks;
using Magnett.Automation.Core.WorkFlows.Implementations;

namespace Magnett.Automation.Core.WorkFlows
{
    public interface IFlow
    {
        Guid Id { get; }

        Task<NodeExit> Run();
    }
}