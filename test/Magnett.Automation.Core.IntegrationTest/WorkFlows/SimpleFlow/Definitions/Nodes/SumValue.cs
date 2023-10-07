using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows;
using Magnett.Automation.Core.WorkFlows.Runtimes;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes;

internal class SumValue : Common
{
    #region ExitCodes

    public class ExitCode : Enumeration
    {
        public static readonly ExitCode Done  = new ExitCode(1, "Done"); 

        private ExitCode(int id, string name) : base(id, name)
        {
        }
    }
        
    #endregion
        
    private SumValue(CommonNamedKey key, ContextDefinition contextDefinition) : 
        base(key, contextDefinition)
    {
    }


    public override NodeExit Execute(Context context)
    {
        var firstDigit  = context.Value(ContextDefinition.FirstDigit);
        var secondDigit = context.Value(ContextDefinition.SecondDigit);
            
        context.Store(ContextDefinition.Result, firstDigit + secondDigit);

        return NodeExit.Create(ExitCode.Done.Name);
    }
        
    public static SumValue Create(CommonNamedKey key, ContextDefinition contextDefinition)
    {
        return new SumValue(key, contextDefinition);
    }
}