namespace Magnett.Automation.Core.WorkFlows.Builders; 

public class NodeLinkBuilder : INodeLinkBuilder, IGotoNodeLinkBuilder
{
    private readonly INodeBase _fromNode;
    private readonly Func<INodeBase, INodeLink, INodeLinkBuilder> _storeAction;
    private readonly Func<FlowDefinitionBuilder> _returnAction;
        
    private string _code;
        
    private NodeLinkBuilder(
        INodeBase fromNode, 
        Func<INodeBase, INodeLink, INodeLinkBuilder> storeAction,
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
            NodeLink.Create(_fromNode.Key, toNodeKey, _code));
    }
    #endregion

    public static INodeLinkBuilder Create(
        INodeBase fromNode,  
        Func<INodeBase, INodeLink, INodeLinkBuilder> storeAction, 
        Func<FlowDefinitionBuilder> returnAction)
    {
        return new NodeLinkBuilder(fromNode, storeAction, returnAction);
    }
}