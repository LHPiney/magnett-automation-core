namespace Magnett.Automation.Core.WorkFlows.Runtimes;

public interface IFlow
{
    Guid Id { get; }
        
    Context Context { get; }

    Task<NodeExit> Run();
}