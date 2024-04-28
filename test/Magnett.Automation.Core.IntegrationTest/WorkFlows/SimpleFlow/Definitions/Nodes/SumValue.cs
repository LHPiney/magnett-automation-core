using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes;

internal class SumValue : Node
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
        
    public SumValue(CommonNamedKey key) : base(key)
    {
    }
    
    public override NodeExit Execute(Context context)
    {
        var firstDigit  = context.Value(ContextDefinition.FirstDigit);
        var secondDigit = context.Value(ContextDefinition.SecondDigit);
            
        context.Store(ContextDefinition.Result, firstDigit + secondDigit);

        return NodeExit.Create(ExitCode.Done.Name);
    }
}