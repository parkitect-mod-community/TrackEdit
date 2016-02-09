using System;

namespace HelloMod
{
    public class FiniteStateMachine
    {
        private IState _currentState;
        public FiniteStateMachine ()
        {
        }

        public void ChangeState(IState newState)
        {
            _currentState = newState;
        }

        public void Update()
        {
            _currentState.Update (this);
        }
    }
}

