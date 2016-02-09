using System;
using UnityEngine;

namespace HelloMod
{
    public class IdleState : IState
    {
        private SharedStateData _stateData;
        public IdleState ()
        {
            _stateData = new SharedStateData ();
        }

        public void Update(FiniteStateMachine stateMachine)
        {
            var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            if (Input.GetMouseButtonDown (0)) {
                RaycastHit hit;
                if (Physics.Raycast (ray, out hit, Mathf.Infinity, -1)) {
                    if (hit.transform.name == "BezierNode") {
                        _stateData.Selected = hit.transform;
                        _stateData.Distance = (ray.origin - hit.point).magnitude;
                        _stateData.FixedY = hit.transform.position.y;

                        _stateData.Offset = hit.transform.position - ray.GetPoint(_stateData.Distance);
             
                    }
                }
                stateMachine.ChangeState (new HorizantalDragState (_stateData));
            }
        }
    
    }
}

