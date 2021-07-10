namespace Magnett.Automation.Core.StateMachine
{
    public interface IState
    {
        string Name { get; }

        public bool CanManageAction(string actionName);

        public ITransition ManageAction(string actionName);

        public bool IsFinalState();
    }
}