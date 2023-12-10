using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes;

internal class ResetValue : Node
{
    #region ExitCodes

    public class ExitCode : Enumeration
    {
        public static readonly ExitCode Ok  = new ExitCode(1, "Ok"); 

        private ExitCode(int id, string name) : base(id, name)
        {
        }
    }
        
    #endregion

    public ResetValue(CommonNamedKey key) : base(key)
    {
            
    }

    public override NodeExit Execute(Context context)
    {
        context.Store(ContextDefinition.FirstDigit, 0);
        context.Store(ContextDefinition.SecondDigit, 0);
        context.Store(ContextDefinition.Result, 0);
            
        return NodeExit.Create(ExitCode.Ok.Name);
    }
}