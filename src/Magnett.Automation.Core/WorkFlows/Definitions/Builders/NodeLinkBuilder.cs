using Magnett.Automation.Core.WorkFlows.Definitions.Implementations;

namespace Magnett.Automation.Core.WorkFlows.Definitions.Builders; 

public class NodeLinkBuilder : INodeLinkBuilder, IGotoNodeLinkBuilder
{
    private readonly CommonNamedKey _fromNode;
    private readonly Func<CommonNamedKey, INodeLink, INodeLinkBuilder> _storeAction;
    private readonly Func<FlowDefinitionBuilder> _returnAction;
        
    private string _code;
        
    private NodeLinkBuilder(
        CommonNamedKey fromNode, 
        Func<CommonNamedKey, INodeLink, INodeLinkBuilder> storeAction,
        Func<FlowDefinitionBuilder> returnAction)
    {
        _fromNode = fromNode
                    ?? throw new ArgumentNullException(nameof(fromNode));
        _storeAction = storeAction
                       ?? throw new ArgumentNullException(nameof(storeAction));
        _returnAction = returnAction 
                        ?? throw new ArgumentNullException(nameof(returnAction));
    }

    #region  INodeLinkBuilder
    public IGotoNodeLinkBuilder OnExitCode(Enumeration code)
    {
        return OnExitCode(code.Name);
    }        
        
    public IGotoNodeLinkBuilder OnExitCode(string code)
    {
        _code = code;

        return this;
    }

    public FlowDefinitionBuilder Build()
    {
        return _returnAction.Invoke();
    }
    #endregion

    #region IGotoNodeLinkBuilder
    public INodeLinkBuilder GoTo(CommonNamedKey toNodeKey)
    {
        return _storeAction.Invoke(
            _fromNode,
            NodeLink.Create(_fromNode, toNodeKey, _code));
    }
    #endregion

    /// <summary>
    /// Creates a new NodeLinkBuilder instance for the specified from node.
    /// </summary>
    /// <param name="fromNode">The source node key.</param>
    /// <param name="storeAction">Function to store the node link configuration.</param>
    /// <param name="returnAction">Function to return to the flow definition builder.</param>
    /// <returns>A new NodeLinkBuilder instance.</returns>
    public static INodeLinkBuilder Create(
        CommonNamedKey fromNode,  
        Func<CommonNamedKey, INodeLink, INodeLinkBuilder> storeAction, 
        Func<FlowDefinitionBuilder> returnAction)
    {
        return new NodeLinkBuilder(fromNode, storeAction, returnAction);
    }
}