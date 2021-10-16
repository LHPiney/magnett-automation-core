using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.WorkFlows.SimpleFlow.Definitions.Nodes
{
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
        

        public override void Execute()
        {
            var firstDigit  = GlobalContext.Value(ContextDefinition.FirstDigit);
            var secondDigit = GlobalContext.Value(ContextDefinition.SecondDigit);
            
            GlobalContext.Store(ContextDefinition.Result, firstDigit + secondDigit);
        }
        
        public static SumValue Create(CommonNamedKey key, ContextDefinition contextDefinition)
        {
            return new SumValue(key, contextDefinition);
        }
    }
}