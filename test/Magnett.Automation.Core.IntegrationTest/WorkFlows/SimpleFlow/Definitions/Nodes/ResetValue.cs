using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes;

internal class ResetValue : Common
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

    private ResetValue(CommonNamedKey key, ContextDefinition contextDefinition) : 
        base(key, contextDefinition)
    {
            
    }

    public override NodeExit Execute(Context context)
    {
        context.Store(ContextDefinition.FirstDigit, 0);
        context.Store(ContextDefinition.SecondDigit, 0);
        context.Store(ContextDefinition.Result, 0);
            
        return NodeExit.Create(ExitCode.Ok.Name);
    }

    public static ResetValue Create(CommonNamedKey name, ContextDefinition contextDefinition)
    {
        return new ResetValue(name, contextDefinition);
    }
}