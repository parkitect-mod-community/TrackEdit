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

			Vector3 position = new Vector3 (point.x, _stateData.FixedY, point.z) + new Vector3 (_stateData.Offset.x, _stateData.OffsetFixedY, _stateData.Offset.z);

			_stateData.Selected.position = position;
           
			var P0BaseNode = this._stateData.SegmentManager.GetFirstSegment ().GetFirstCurve.P0;
			if (_stateData.Selected.gameObject.GetComponent<TrackCurveNode> ().NodePoint == TrackCurveNode.NodeType.P3 && (position - P0BaseNode.GetGlobal()).sqrMagnitude < .2f) {
				_stateData.Selected.position = P0BaseNode.GetGlobal ();

				if (Input.GetMouseButtonUp (0)) {

					this._stateData.SegmentManager.ConnectEndPieces (_stateData.Selected.gameObject.GetComponent<TrackCurveNode> ().TrackSegmentModify, this._stateData.SegmentManager.GetFirstSegment ());
					//P0BaseNode.TrackSegmentModify.TrackSegment.initiateFromPreviousSegment (_stateData.Selected.gameObject.GetComponent<TrackCurveNode> ().TrackSegmentModify.TrackSegment);
					stateMachine.ChangeState(new IdleState (_stateData.SegmentManager));
				}
			}

			_stateData.Selected.gameObject.GetComponent<TrackCurveNode>().NodeUpdate();


            if (Input.GetKeyDown (Main.Configeration.VerticalKey)) {
                _stateData.OffsetFixedY = _stateData.Selected.transform.position.y - point.y;

                stateMachine.ChangeState (new VerticalDragState (_stateData));
            }
            if (Input.GetMouseButtonUp (0)) {
				stateMachine.ChangeState(new IdleState (_stateData.SegmentManager));
            }
        }
    }
}

