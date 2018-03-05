namespace TrackEdit.StateMachine
{
    public interface IState
    {
        void Update(FiniteStateMachine stateMachine);
        void Unload();
    }
}