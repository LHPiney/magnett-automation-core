using System.Threading;

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

    public async Task<NodeExit> Run(CancellationToken cancellationToken = default)
    {
        return await _flowRunner.Start(cancellationToken);
    }

    #endregion  
        
    /// <summary>
    /// Creates a new Flow instance with the specified flow runner.
    /// </summary>
    /// <param name="flowRunner">The flow runner to execute the flow.</param>
    /// <returns>A new Flow instance.</returns>
    public static IFlow Create(IFlowRunner flowRunner)
    {
        return new Flow(flowRunner);
    }
}