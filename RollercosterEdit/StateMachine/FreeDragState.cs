using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class FreeDragState : IState
    {
        private bool _verticalDragState;
        private SharedStateData _stateData;
        public FreeDragState (SharedStateData stateData)
        {
            this._stateData = stateData;
        }

        public void Update(FiniteStateMachine stateMachine)
        {
            var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            Vector3 point = ray.GetPoint (_stateData.Distance);
            Vector3 position = Vector3.zero;
            if (!_verticalDragState) {
                position = new Vector3 (point.x, _stateData.FixedY, point.z) + new Vector3(_stateData.Offset.x, _stateData.Offset.y, _stateData.Offset.z);

            } else
            {
                _stateData.FixedY = point.y;
                position = new Vector3 (_stateData.Selected.position.x, _stateData.FixedY, _stateData.Selected.position.z) + new Vector3(0, _stateData.Offset.y, 0);
            }

            TrackNode trackNode = _stateData.Selected.gameObject.GetComponent<TrackNode> ();

            if (Input.GetKeyDown (Main.Configeration.VerticalKey)) {
                _stateData.Offset = new Vector3(_stateData.Offset.x,_stateData.Selected.transform.position.y - point.y, _stateData.Offset.z);
                _verticalDragState = true;
 
            } else if (Input.GetKeyUp (Main.Configeration.VerticalKey)) {
                _verticalDragState = false;
                _stateData.Offset = (_stateData.Selected.transform.position - point);

            }

            var nextSegment = trackNode.TrackSegmentModify.GetNextSegment ();
            var previousSegment = trackNode.TrackSegmentModify.GetPreviousSegment ();

            switch (trackNode.NodePoint) {
            case TrackNode.NodeType.PO:
                break;
            case TrackNode.NodeType.P1:
                trackNode.SetPoint (position);

                if (previousSegment != null) {

                    trackNode.CalculateLenghtAndNormals ();


                    float magnitude = Mathf.Abs((previousSegment.GetLastCurve.P2.GetGlobal () - previousSegment.GetLastCurve.P3.GetGlobal ()).magnitude);
                    previousSegment.GetLastCurve.P2.SetPoint (previousSegment.GetLastCurve.P3.GetGlobal() + (trackNode.TrackSegmentModify.TrackSegment.getTangentPoint(0f) *-1f* magnitude));
                    previousSegment.Invalidate = true;
                    trackNode.CalculateLenghtAndNormals ();

                    trackNode.TrackSegmentModify.CalculateStartBinormal ();
                    //TrackSegmentModify.TrackSegment.upgradeSavegameRecalculateBinormal (previousSegment.TrackSegment);


                }


                break;
            case TrackNode.NodeType.P2:

                trackNode.SetPoint (position);

                if (nextSegment != null) {

                    trackNode.CalculateLenghtAndNormals ();


                    float magnitude = Mathf.Abs((nextSegment.GetFirstCurve.P0.GetGlobal () - nextSegment.GetFirstCurve.P1.GetGlobal ()).magnitude);
                    nextSegment.GetFirstCurve.P1.SetPoint (nextSegment.GetFirstCurve.P0.GetGlobal () + (trackNode.TrackSegmentModify.TrackSegment.getTangentPoint(1f) * magnitude));
                    nextSegment.Invalidate = true;
                    trackNode.CalculateLenghtAndNormals ();

                    nextSegment.CalculateStartBinormal ();

                }


                break;
            case TrackNode.NodeType.P3:

                if (trackNode.TrackCurve.Group == TrackNodeCurve.Grouping.End || trackNode.TrackCurve.Group == TrackNodeCurve.Grouping.Both ) {

                    if (nextSegment != null) {

                        var NextP1Offset = nextSegment.GetFirstCurve.P1.GetGlobal() -trackNode.GetGlobal();
                        var P2Offset = trackNode.TrackCurve.P2.GetGlobal () - trackNode.GetGlobal ();


                        nextSegment.GetFirstCurve.P0.SetPoint (position);
                        nextSegment.GetFirstCurve.P1.SetPoint (position+ NextP1Offset);
                        trackNode.TrackCurve.P2.SetPoint(position+ P2Offset);

                        nextSegment.Invalidate = true;
                    }

                }
                trackNode.SetPoint (position);

                break;
            }

          //  _stateData.Selected.position = position;

            trackNode.TrackSegmentModify.Invalidate = true;
            trackNode.Validate ();
            
           
	//		var P0BaseNode = this._stateData.SegmentManager.GetFirstSegment ().GetFirstCurve.P0;
	//		if (_stateData.Selected.gameObject.GetComponent<TrackNode> ().NodePoint == TrackNode.NodeType.P3 && (position - P0BaseNode.GetGlobal()).sqrMagnitude < .2f) {
	//			_stateData.Selected.position = P0BaseNode.GetGlobal ();

	//			if (Input.GetMouseButtonUp (0)) {

	//				this._stateData.SegmentManager.ConnectEndPieces (_stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify, this._stateData.SegmentManager.GetFirstSegment ());
					//P0BaseNode.TrackSegmentModify.TrackSegment.initiateFromPreviousSegment (_stateData.Selected.gameObject.GetComponent<TrackCurveNode> ().TrackSegmentModify.TrackSegment);
	//				stateMachine.ChangeState(new IdleState (_stateData.SegmentManager));
	//			}
		//	}



            if (Input.GetMouseButtonUp (0)) {
				stateMachine.ChangeState(new IdleState (_stateData.SegmentManager));
            }
        }
    }
}

