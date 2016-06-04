using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class FreeDragState : DraggableState
    {
		private BuilderHeightMarker _heightMaker;
    
       private SharedStateData _stateData;

        public FreeDragState (SharedStateData stateData) : base(stateData)
        {
			
			this._stateData = stateData;
			_heightMaker = UnityEngine.Object.Instantiate<BuilderHeightMarker>(ScriptableSingleton<AssetManager>.Instance.builderHeightMarkerGO);
			_heightMaker.attachedTo = stateData.Selected.transform;
			_heightMaker.heightChangeDelta = .1f;


        }

        public override void Update(FiniteStateMachine stateMachine)
        {

            base.Update (stateMachine);

            TrackNode trackNode = _stateData.Selected.gameObject.GetComponent<TrackNode> ();


            //calculate the match for the node segment
            TrackNodeHelper.CalculateMatch (trackNode, dragPosition);

            
			var nextSegment = trackNode.TrackSegmentModify.GetNextSegment (false);
			if (!_stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.TrackSegment.isConnectedToNextSegment) {
			
			
                if (_stateData.Selected.gameObject.GetComponent<TrackNode> ().NodePoint == TrackNode.NodeType.P3 && (dragPosition - nextSegment.GetFirstCurve.P0.GetGlobal ()).sqrMagnitude < .2f) {
					
					float magnitude = Mathf.Abs ((nextSegment.GetFirstCurve.P0.GetGlobal () - nextSegment.GetFirstCurve.P1.GetGlobal ()).magnitude);


					_stateData.Selected.gameObject.GetComponent<TrackNode> ().SetPoint (nextSegment.GetFirstCurve.P0.GetGlobal ());
					_stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.GetLastCurve.P2.SetPoint (_stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.GetLastCurve.P3.GetGlobal () + (nextSegment.TrackSegment.getTangentPoint (0f) * -1f * magnitude));
					_stateData.Selected.gameObject.GetComponent<TrackNode> ().CalculateLenghtAndNormals ();
					_stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.GetFirstCurve.P0.TrackSegmentModify.CalculateStartBinormal (false);
					_stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.GetLastCurve.ClearExtrudeNode ();


                    nextSegment.GetFirstCurve.P0.CalculateLenghtAndNormals ();
					if (Input.GetMouseButtonUp (0)) {
                        _stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.ConnectWithForwardSegment (nextSegment);
                        stateMachine.ChangeState (new IdleState (_stateData));
					}
					nextSegment.Invalidate = true;

					_stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.Invalidate = true;

				}
			}

            if (Input.GetMouseButtonUp (0)) {
				stateMachine.ChangeState(new IdleState (_stateData));
            }
        }

        public override void Unload()
		{
            base.Unload ();
			UnityEngine.Object.Destroy (_heightMaker.gameObject);
		}

    }
}

