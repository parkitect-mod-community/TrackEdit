using System;
using UnityEngine;

namespace HelloMod
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
            _stateData.Selected.position = new Vector3 (point.x, _stateData.FixedY, point.y) + _stateData.Offset;

            if (Input.GetKeyDown (Main.Configeration.VerticalKey)) {
                stateMachine.ChangeState (new VerticalDragState (_stateData));
            }
            if (Input.GetMouseButtonUp (0)) {
                stateMachine.ChangeState(new IdleState ());
            }
        }
    }
}

