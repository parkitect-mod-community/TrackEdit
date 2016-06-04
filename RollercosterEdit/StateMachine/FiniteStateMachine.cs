using System;

namespace RollercoasterEdit
{
    public class FiniteStateMachine
    {
        private IState _currentState;
        public FiniteStateMachine ()
        {
        }

        public void ChangeState(IState newState)
        {
			if(_currentState != null)
			    _currentState.Unload ();
            _currentState = newState;
        }

        public void Update()
        {
            if(_currentState != null)
            _currentState.Update (this);
        }

        public void Unload()
        {
            _currentState.Unload ();
            _currentState = null;
        }
    }
}

