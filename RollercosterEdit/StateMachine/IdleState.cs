using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class IdleState : IState
    {
        private SharedStateData stateData;
		public IdleState (SharedStateData stateData)
        {
            this.stateData = new SharedStateData ();
            this.stateData.SetActiveNode(stateData.ActiveNode);
		}

        public void Update(FiniteStateMachine stateMachine)
        {


            var ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			if (Input.GetMouseButtonDown (1)) {
				RaycastHit hit;
				if (Physics.Raycast (ray, out hit, Mathf.Infinity, LayerMasks.COASTER_TRACKS)) {
					if (hit.transform.name == "BezierNode") {
						stateData.SetActiveNode(hit.transform);
					}
				}
			}

            if (Input.GetMouseButtonDown (0)) {
                RaycastHit hit;

				if (Physics.Raycast (ray, out hit, Mathf.Infinity,LayerMasks.COASTER_TRACKS)) {
					stateData.Selected = hit.transform;
					stateData.FixedY = hit.transform.position.y;
					stateData.Offset = new Vector3(hit.transform.position.x - hit.point.x, 0, hit.transform.position.z - hit.point.z);

					stateData.Distance = (ray.origin - hit.point).magnitude;

                    if (hit.transform.name == "BezierNode") {
                        stateData.SetActiveNode (hit.transform);

                        TrackNode node = hit.transform.GetComponent<TrackNode> ();

                        var nextSegment = node.trackSegmentModify.GetNextSegment (true);
                        var previousSegment = node.trackSegmentModify.GetPreviousSegment (true);

                        if (node.nodePoint == TrackNode.NodeType.P1 && previousSegment != null && previousSegment.TrackSegment is Station) {
                            stateMachine.ChangeState (new LinearDragState (stateData));
                        } else if (node.nodePoint == TrackNode.NodeType.P2 && nextSegment != null && nextSegment.TrackSegment is Station) {
                            stateMachine.ChangeState (new LinearDragState (stateData));
                        } else {
                            stateMachine.ChangeState (new FreeDragState (stateData));
                        }
                    } else if (hit.transform.name == "ExtrudeNode") {
                        stateMachine.ChangeState (new ConsumeExtrudeNodeState (stateData));
                    } else if (hit.transform.name == "Rotate") {
                        stateMachine.ChangeState (new RotationState (stateData));
                    }
                }
              
            }
        }

		public void Unload()
		{
		}
    
    }
}

