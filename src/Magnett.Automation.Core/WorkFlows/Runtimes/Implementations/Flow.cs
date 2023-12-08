namespace Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

public class Flow : IFlow
{ 
    private readonly IFlowRunner _flowRunner;

    private Flow(IFlowRunner flowRunner)
    {
        _flowRunner = flowRunner 
                      ?? throw new ArgumentNullException(nameof(flowRunner));
    }

    #region IFlow

    public Guid Id { get; } = Guid.NewGuid();
        
    public Context  Context => _flowRunner.Context;

    public async Task<NodeExit> Run()
    {
        return await _flowRunner.Start();
    }

    #endregion  
        
    public static IFlow Create(IFlowRunner flowRunner)
    {
        return new Flow(flowRunner);
    }
}