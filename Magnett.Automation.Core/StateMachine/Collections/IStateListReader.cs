namespace Magnett.Automation.Core.StateMachine.Collections
{
    public interface IStateListReader
    {
        bool HasState(string stateName);

        int Count();
    }
}