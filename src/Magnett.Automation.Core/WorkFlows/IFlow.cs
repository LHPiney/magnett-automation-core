using System;
using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows.Implementations;

namespace Magnett.Automation.Core.WorkFlows
{
    public interface IFlow
    {
        Guid Id { get; }
        
        Context Context { get; }

        Task<NodeExit> Run();
    }
}