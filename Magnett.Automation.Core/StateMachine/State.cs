using System;
using System.Runtime.CompilerServices;

using Magnett.Automation.Core.StateMachine.Collections;
using Magnett.Automation.Core.StateMachine.Exceptions;

[assembly: InternalsVisibleTo("Magnett.Automation.Core.Test")]
namespace Magnett.Automation.Core.StateMachine
{
    internal class State : IState
    {
        private readonly TransitionList _transitions;
        
        public string Name { get; private init; }

        private State(string name)
        {
            Name = name 
                   ?? throw new ArgumentNullException(nameof(name));
            
            _transitions = TransitionList.Create();
        }

        #region IState

        public bool CanManageAction(string actionName)
        {
            return _transitions.HasItem(actionName);
        }

        public ITransition ManageAction(string actionName)
        {
            return CanManageAction(actionName)
                ? _transitions.Get(actionName)
                : throw new ActionNotFoundException(Name, actionName);
        }

        public bool IsFinalState()
        {
            return _transitions.Count == 0;
        }

        #endregion
        
        public static State Create(string name)
        {
            return new(name);
        }
    }
}