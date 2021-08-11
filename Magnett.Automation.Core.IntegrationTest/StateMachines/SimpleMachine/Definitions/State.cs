using Magnett.Automation.Core.Commons;

namespace Magnett.Automation.Core.IntegrationTest.StateMachines.SimpleMachine.Definitions
{
    public class State : Enumeration
    {
        public static readonly State Init = new State(1, "Init");
        public static readonly State Working = new State(2, "Working");
        public static readonly State Paused = new State(3, "Paused");
        public static readonly State Finished = new State(4, "Finished");
        
        private State(int id, string name) : base(id, name)
        {
            
        }
    }
}