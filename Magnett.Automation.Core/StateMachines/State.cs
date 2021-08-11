using System;
using System.Runtime.CompilerServices;
using Magnett.Automation.Core.Commons;
using Magnett.Automation.Core.StateMachines.Collections;
using Magnett.Automation.Core.StateMachines.Exceptions;

[assembly: InternalsVisibleTo("Magnett.Automation.Core.Test")]
namespace Magnett.Automation.Core.StateMachines
{
    internal class State : IState
    {
        private readonly TransitionList _transitionList;
        
        public CommonNamedKey Key { get; }

        private State(string name, TransitionList transitionList)
        {
            Key = CommonNamedKey.Create(name);

            _transitionList = transitionList
                              ?? throw new ArgumentNullException(nameof(transitionList));
        }
        
        private bool CanManageAction(CommonNamedKey actionName)
        {
            return _transitionList
                .HasItem(actionName);
        }

        #region IState
        public ITransition ManageAction(CommonNamedKey actionName)
        {
            return CanManageAction(actionName)
                ? _transitionList.Get(actionName)
                : throw new ActionNotFoundException(Key.Name, actionName.Name);
        }

        public bool IsFinalState()
        {
            return _transitionList.IsEmpty();
        }

        #endregion
        
        public static State Create(string name, TransitionList transitionList)
        {
            return new(name, transitionList);
        }
    }
}