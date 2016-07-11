using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class FreeDragState : DraggableState
    {
        private BuilderHeightMarker heightMaker;
        private BuilderHeightMarker p3HeightMarker;
        private SharedStateData stateData;

        public FreeDragState (SharedStateData stateData) : base(stateData)
        {
			
			this.stateData = stateData;
			heightMaker = UnityEngine.Object.Instantiate<BuilderHeightMarker>(ScriptableSingleton<AssetManager>.Instance.builderHeightMarkerGO);
			heightMaker.attachedTo = stateData.Selected.transform;
			heightMaker.heightChangeDelta = .1f;


            TrackNode trackNode = stateData.Selected.gameObject.GetComponent<TrackNode> ();
            var previousSegment = trackNode.trackSegmentModify.GetPreviousSegment (true);
            Transform p3hook = null;

            if (trackNode.nodePoint == TrackNode.NodeType.P1) {
                p3hook = previousSegment.GetLastCurve.P3.transform;

            } else if (trackNode.nodePoint == TrackNode.NodeType.P2) {   
                p3hook = trackNode.trackCurve.P3.transform;
            }

            if (p3hook != stateData.Selected.transform && p3hook != null) {
                p3HeightMarker = UnityEngine.Object.Instantiate<BuilderHeightMarker> (ScriptableSingleton<AssetManager>.Instance.builderHeightMarkerGO);
                p3HeightMarker.attachedTo = p3hook;
                p3HeightMarker.heightChangeDelta = .01f;
            }
            


        }

        public override void Update(FiniteStateMachine stateMachine)
        {

            base.Update (stateMachine);

            TrackNode trackNode = stateData.Selected.gameObject.GetComponent<TrackNode> ();


            //calculate the match for the node segment
            TrackNodeHelper.CalculateMatch (trackNode, dragPosition);

            
			var nextSegment = trackNode.trackSegmentModify.GetNextSegment (false);
			if (!stateData.Selected.gameObject.GetComponent<TrackNode> ().trackSegmentModify.TrackSegment.isConnectedToNextSegment) {
			
			
                if (stateData.Selected.gameObject.GetComponent<TrackNode> ().nodePoint == TrackNode.NodeType.P3 && (dragPosition - nextSegment.GetFirstCurve.P0.GetGlobal ()).sqrMagnitude < .2f) {
					
					float magnitude = Mathf.Abs ((nextSegment.GetFirstCurve.P0.GetGlobal () - nextSegment.GetFirstCurve.P1.GetGlobal ()).magnitude);


					stateData.Selected.gameObject.GetComponent<TrackNode> ().SetPoint (nextSegment.GetFirstCurve.P0.GetGlobal ());
					stateData.Selected.gameObject.GetComponent<TrackNode> ().trackSegmentModify.GetLastCurve.P2.SetPoint (stateData.Selected.gameObject.GetComponent<TrackNode> ().trackSegmentModify.GetLastCurve.P3.GetGlobal () + (nextSegment.TrackSegment.getTangentPoint (0f) * -1f * magnitude));
					stateData.Selected.gameObject.GetComponent<TrackNode> ().CalculateLenghtAndNormals ();
					stateData.Selected.gameObject.GetComponent<TrackNode> ().trackSegmentModify.GetFirstCurve.P0.trackSegmentModify.CalculateStartBinormal (false);
					stateData.Selected.gameObject.GetComponent<TrackNode> ().trackSegmentModify.GetLastCurve.ClearExtrudeNode ();


                    nextSegment.GetFirstCurve.P0.CalculateLenghtAndNormals ();
					if (Input.GetMouseButtonUp (0)) {
                        stateData.Selected.gameObject.GetComponent<TrackNode> ().trackSegmentModify.ConnectWithForwardSegment (nextSegment);
                        stateMachine.ChangeState (new IdleState (stateData));
					}
					nextSegment.invalidate = true;

					stateData.Selected.gameObject.GetComponent<TrackNode> ().trackSegmentModify.invalidate = true;

				}
			}

            if (Input.GetMouseButtonUp (0)) {
				stateMachine.ChangeState(new IdleState (stateData));
            }
        }

        public override void Unload()
		{
            base.Unload ();
			UnityEngine.Object.Destroy (heightMaker.gameObject);
            if (p3HeightMarker != null)
                UnityEngine.Object.Destroy (p3HeightMarker.gameObject);
		}

    }
}

