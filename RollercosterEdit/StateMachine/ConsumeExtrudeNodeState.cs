using System;
using UnityEngine;

namespace RollercoasterEdit
{
	public class ConsumeExtrudeNodeState : IState
	{
        private SharedStateData stateData;

		public ConsumeExtrudeNodeState (SharedStateData stateData)
		{
			this.stateData = stateData;
		}

		public void Update(FiniteStateMachine stateMachine)
		{

            //the old distance between the p2-p3 node
            TrackNodeCurve curve =  stateData.Selected.GetComponent<ExtrudeNode> ().trackSegmentModify.GetLastCurve;
            Vector3 direction = curve.P2.GetGlobal () - curve.P3.GetGlobal ();

			var trackSegment = UnityEngine.Object.Instantiate<TrackSegment4>( ScriptableSingleton<AssetManager>.Instance.getPrefab<TrackSegment4>(Prefabs.Straight));

            TrackUIHandle.instance.trackRide.Track.addSegment (trackSegment, stateData.Selected.GetComponent<ExtrudeNode> ().trackSegmentModify.GetIndexOfSegment ()+1);
			trackSegment.Initialize ();


            var modify = trackSegment.gameObject.AddComponent<TrackSegmentModify> ();
            stateData.Selected.GetComponent<ExtrudeNode> ().trackSegmentModify.ConnectWithForwardSegment(modify);


            //_stateData.Selected.GetComponent<ExtrudeNode>().TrackSegmentModify.GetLastCurve.P3

            modify.CalculateStartBinormal (true);
                
            stateData.Selected = modify.GetLastCurve.P3.gameObject.transform;

            //re-apply p2-p3 because that is lost when the segments are merged
            TrackNodeHelper.CalculateMatch (curve.P2, curve.P3.GetGlobal() + direction);

            stateMachine.ChangeState(new FreeDragWithSmoothing(stateData));

			//stateMachine.ChangeState (new IdleState (_stateData));
		}


		public void Unload()
		{
			
		}
	}
}

