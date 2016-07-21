using System;

namespace RollercoasterEdit
{
    public class FiniteStateMachine
    {
        private IState currentState;
        public FiniteStateMachine ()
        {
        }

        public void ChangeState(IState newState)
        {
			if(currentState != null)
			    currentState.Unload ();
            currentState = newState;
        }

        public void Update()
        {
            if(currentState != null)
            currentState.Update (this);
        }

        public void Unload()
        {
            if(currentState != null)
                currentState.Unload ();
            currentState = null;
        }
    }
}

