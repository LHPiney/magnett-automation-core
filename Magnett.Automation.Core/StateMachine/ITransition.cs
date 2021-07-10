namespace Magnett.Automation.Core.StateMachine
{
    public interface ITransition
    {
        public string ToStateName { get; }
        public string Action { get; } 
    }
}