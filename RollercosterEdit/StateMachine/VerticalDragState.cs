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
    
            _stateData.Selected.position = new Vector3 ( _stateData.Selected.position.x, _stateData.FixedY, _stateData.Selected.position.z) + new Vector3(0,_stateData.Offset.y,0) ;
            
           // _stateData.Selected.gameObject.GetComponent<TrackNode>().NodeUpdate();


            if (Input.GetKeyUp(Main.Configeration.VerticalKey)) {

                _stateData.Offset = (_stateData.Selected.transform.position - point);
                _stateData.OffsetFixedY = _stateData.Selected.transform.position.y - point.y;

                stateMachine.ChangeState (new FreeDragState (_stateData));
            }
            if (Input.GetMouseButtonUp (0)) {
				stateMachine.ChangeState(new IdleState (_stateData.SegmentManager));
            }
        }
    }
}

