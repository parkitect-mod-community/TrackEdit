using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class IdleState : IState
    {
        private bool _verticalState= false;
        private SharedStateData _stateData;
		public IdleState (TrackSegmentManager segmentManager)
        {
            _stateData = new SharedStateData ();
			_stateData.SegmentManager = segmentManager;
		}

        public void Update(FiniteStateMachine stateMachine)
        {
            if (Input.GetKeyDown(Main.Configeration.VerticalKey))
            {
                _verticalState = true;
            }
            else if (Input.GetKeyUp(Main.Configeration.VerticalKey))
            {
                _verticalState = false;
            }
                var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Input.GetMouseButtonDown (0)) {
                RaycastHit hit;
                if (Physics.Raycast (ray, out hit, Mathf.Infinity, -1)) {
                    if (hit.transform.name == "BezierNode") {
                        _stateData.Selected = hit.transform;
                        _stateData.FixedY = hit.transform.position.y;
                        _stateData.Offset = hit.transform.position - hit.point;
                        
                        _stateData.Distance = (ray.origin - hit.point).magnitude;

                        if (_verticalState)
                        {
                            stateMachine.ChangeState(new VerticalDragState(_stateData));
                        }
                        else
                        stateMachine.ChangeState(new HorizantalDragState(_stateData));
                    }
                }
              
            }
        }
    
    }
}

