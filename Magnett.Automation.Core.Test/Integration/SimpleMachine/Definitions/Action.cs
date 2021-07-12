using Magnett.Automation.Core.Common;

namespace Magnett.Automation.Core.Test.Integration.SimpleMachine.Definitions
{
    public class Action : Enumeration
    {
        public static readonly Action Start = new Action(1, "Start");
        public static readonly Action Pause = new Action(2, "Pause");
        public static readonly Action Continue = new Action(3, "Continue");
        public static readonly Action Finish = new Action(4, "Finish");
            
        private Action(int id, string name) : base(id, name)
        {
            
        }
    }
}