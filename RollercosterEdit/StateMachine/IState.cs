using System;

namespace RollercoasterEdit
{
    public interface IState
    {
        void Update(FiniteStateMachine stateMachine);
		void Unload();
	}

}

