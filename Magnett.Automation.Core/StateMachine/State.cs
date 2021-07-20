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
        
        public string Name { get; }

        private State(string name, TransitionList transitions)
        {
            Name = name 
                   ?? throw new ArgumentNullException(nameof(name));

            _transitions = transitions
                           ?? throw new ArgumentNullException(nameof(transitions));
        }
        
        private bool CanManageAction(string actionName)
        {
            return _transitions.HasItem(actionName);
        }

        #region IState
        public ITransition ManageAction(string actionName)
        {
            return CanManageAction(actionName)
                ? _transitions.GetItem(actionName)
                : throw new ActionNotFoundException(Name, actionName);
        }

        public bool IsFinalState()
        {
            return _transitions.IsEmpty();
        }

        #endregion
        
        public static State Create(string name, TransitionList transitions)
        {
            return new(name, transitions);
        }
    }
}