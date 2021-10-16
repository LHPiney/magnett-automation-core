using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes
{
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

        public override void Execute()
        {
            GlobalContext.Store(ContextDefinition.FirstDigit, 0);
            GlobalContext.Store(ContextDefinition.SecondDigit, 0);
            GlobalContext.Store(ContextDefinition.Result, 0);
        }

        public static ResetValue Create(CommonNamedKey name, ContextDefinition contextDefinition)
        {
            return new ResetValue(name, contextDefinition);
        }
    }
}