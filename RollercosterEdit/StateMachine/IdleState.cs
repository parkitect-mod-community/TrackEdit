using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class IdleState : IState
    {
        private SharedStateData _stateData;
		public IdleState (TrackSegmentManager segmentManager)
        {
            _stateData = new SharedStateData ();
			_stateData.SegmentManager = segmentManager;
		}

        public void Update(FiniteStateMachine stateMachine)
        {


            var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Input.GetMouseButtonDown (0)) {
                RaycastHit hit;
                if (Physics.Raycast (ray, out hit, Mathf.Infinity, -1)) {
                    if (hit.transform.name == "BezierNode") {
                        _stateData.Selected = hit.transform;
                        _stateData.FixedY = hit.transform.position.y;
                        _stateData.Offset = new Vector3(hit.transform.position.x - hit.point.x, 0, hit.transform.position.z - hit.point.z);
                        
                        _stateData.Distance = (ray.origin - hit.point).magnitude;
                        
                        stateMachine.ChangeState(new FreeDragState(_stateData));
                    }
                }
              
            }
        }
    
    }
}

