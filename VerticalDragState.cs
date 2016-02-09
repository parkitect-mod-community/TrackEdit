using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class VerticalDragState : IState
    {
        private SharedStateData _stateData;
        public VerticalDragState (SharedStateData stateData)
        {
            this._stateData = stateData;
        }

        public void Update(FiniteStateMachine stateMachine)
        {
            var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            Vector3 point = ray.GetPoint (_stateData.Distance);
            _stateData.FixedY = point.y;
            _stateData.Selected.position = new Vector3 ( _stateData.Selected.position.x, _stateData.FixedY,  _stateData.Selected.position.z);

            _stateData.Selected.gameObject.GetComponent<TrackCurveNode>().NodeUpdate();


            if (Input.GetKeyUp(Main.Configeration.VerticalKey)) {
                _stateData.Offset = (_stateData.Selected.transform.position - point);

                stateMachine.ChangeState (new HorizantalDragState (_stateData));
            }
            if (Input.GetMouseButtonUp (0)) {
                stateMachine.ChangeState(new IdleState ());
            }
        }
    }
}

