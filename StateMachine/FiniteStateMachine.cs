namespace TrackEdit.StateMachine
{
    public class FiniteStateMachine
    {
        private IState _currentState;

        public IState getCurrentState()
        {
            return _currentState;
        }

        public void ChangeState(IState newState)
        {
            if (_currentState != null)
                _currentState.Unload();
            _currentState = newState;
        }

        public void Update()
        {
            if (_currentState != null)
                _currentState.Update(this);
        }

        public void Unload()
        {
            if (_currentState != null)
                _currentState.Unload();
            _currentState = null;
        }
    }
}