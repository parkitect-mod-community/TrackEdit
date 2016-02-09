using System;

namespace HelloMod
{
    public interface IState
    {
        void Update(FiniteStateMachine stateMachine);
    }
}

