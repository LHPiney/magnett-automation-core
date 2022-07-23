namespace Magnett.Automation.Core.WorkFlows;

public interface IFlow
{
    Guid Id { get; }
        
    Context Context { get; }

    Task<NodeExit> Run();
}