using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class FreeDragWithSmoothing : DraggableState
    {
        private SharedStateData stateData;
        private BuilderHeightMarker heightMarker;

        public FreeDragWithSmoothing (SharedStateData stateData) : base(stateData)
        {
            this.stateData = stateData;

            heightMarker = UnityEngine.Object.Instantiate<BuilderHeightMarker>(ScriptableSingleton<AssetManager>.Instance.builderHeightMarkerGO);
            heightMarker.attachedTo = stateData.Selected.transform;
            heightMarker.heightChangeDelta = .01f;

        }

        public override void Update (FiniteStateMachine stateMachine)
        {
            TrackNode trackNode = stateData.Selected.gameObject.GetComponent<TrackNode> ();
            base.Update (stateMachine);

            //calculate the match for the node segment
            TrackNodeHelper.CalculateMatch (trackNode, dragPosition);
            TrackNodeCurve previousCurve = trackNode.trackSegmentModify.getPreviousCurve (trackNode.trackCurve);

            float dist = Vector3.Distance (previousCurve.P3.GetGlobal (), trackNode.trackCurve.P3.GetGlobal ());
            Vector3 dir  = (trackNode.trackCurve.P3.GetGlobal () - previousCurve.P3.GetGlobal ()).normalized;

            TrackNodeHelper.CalculateMatch (trackNode.trackCurve.P1,trackNode.trackCurve.P0.GetGlobal() +  (dist/2.0f) * (trackNode.trackCurve.P1.GetGlobal() - trackNode.trackCurve.P0.GetGlobal()).normalized);


           
            Vector3 p0 = trackNode.trackCurve.P1.GetGlobal() - trackNode.trackCurve.P0.GetGlobal();
           
            Vector3 normal = Vector3.Cross (dir, p0).normalized;
            Vector3 p1final = - (Quaternion.AngleAxis (Vector3.Angle(-dir, p0),normal) * -dir).normalized;

            TrackNodeHelper.CalculateMatch (trackNode.trackCurve.P2,trackNode.trackCurve.P3.GetGlobal() +  (dist/2.0f) * p1final);

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

        public override void Unload ()
        {
            base.Unload ();
            UnityEngine.Object.Destroy (heightMarker.gameObject);
        }


    }
}

