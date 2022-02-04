using System;
using System.Threading.Tasks;
using Magnett.Automation.Core.Contexts;

namespace Magnett.Automation.Core.WorkFlows.Implementations
{
    public abstract class FlowRunnerBase : IFlowRunner
    {
        protected IFlowDefinition Definition { get; }
        protected INodeBase NodeToRun { get; set; }

        protected FlowRunnerBase(IFlowDefinition definition, Context context)
        {
            Definition = definition 
                          ?? throw new ArgumentNullException(nameof(definition));

            FlowContext = context 
                          ?? throw new  ArgumentNullException(nameof(context));
            
            NodeToRun = Definition.InitialNode;
        }

        public Context FlowContext { get; }

        protected async Task<NodeExit> ExecuteNode(INodeBase node)
        {
            return node switch
            {
                INodeAsync nodeAsync => await nodeAsync.Execute(FlowContext),
                INode      nodeSync  => await Task.Run(() => nodeSync.Execute(FlowContext)),
                { }                  => throw new ArgumentException("Not a valid node"),
                null                 => throw new ArgumentNullException(nameof(node))
            };
        }

        public abstract Task<NodeExit> Start();
    }
}