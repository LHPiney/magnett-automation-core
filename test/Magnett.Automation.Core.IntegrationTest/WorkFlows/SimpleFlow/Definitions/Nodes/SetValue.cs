using System;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.Contexts;
using Magnett.Automation.Core.WorkFlows.Runtimes;
using Magnett.Automation.Core.WorkFlows.Runtimes.Implementations;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes;

internal class   SetValue : Node
{
    #region ExitCodes

    public class ExitCode : Enumeration
    {
        public static readonly ExitCode Assigned  = new ExitCode(1, "Assigned"); 

        private ExitCode(int id, string name) : base(id, name)
        {
        }
    }
        
    #endregion
        
    public SetValue(CommonNamedKey key) : base(key)
    {
    }
    
    public override NodeExit Execute(Context context)
    {
        var random = new Random();
            
        context.Store(ContextDefinition.FirstDigit, random.Next(1000));
        context.Store(ContextDefinition.SecondDigit, random.Next(1000));

        return NodeExit.Create(ExitCode.Assigned.Name);
    }
}