using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class HorizantalDragState : IState
    {
        private SharedStateData _stateData;
        public HorizantalDragState (SharedStateData stateData)
        {
            this._stateData = stateData;
        }

        public void Update(FiniteStateMachine stateMachine)
        {
            var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            Vector3 point = ray.GetPoint (_stateData.Distance);

            _stateData.Selected.position = new Vector3(point.x, _stateData.FixedY, point.z) + new Vector3(_stateData.Offset.x,0, _stateData.Offset.z);
            _stateData.Selected.gameObject.GetComponent<TrackCurveNode>().NodeUpdate();

            if (Input.GetKeyDown (Main.Configeration.VerticalKey)) {
                _stateData.Offset = (_stateData.Selected.transform.position - point);

                stateMachine.ChangeState (new VerticalDragState (_stateData));
            }
            if (Input.GetMouseButtonUp (0)) {
                stateMachine.ChangeState(new IdleState ());
            }
        }
    }
}

