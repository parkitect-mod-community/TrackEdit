using System;
using UnityEngine;

namespace RollercoasterEdit
{
	public class ConsumeExtrudeNodeState : IState
	{
		private SharedStateData _stateData;

		public ConsumeExtrudeNodeState (SharedStateData stateData)
		{
			this._stateData = stateData;
		}

		public void Update(FiniteStateMachine stateMachine)
		{

            //the old distance between the p2-p3 node
            TrackNodeCurve curve =  _stateData.Selected.GetComponent<ExtrudeNode> ().TrackSegmentModify.GetLastCurve;
            Vector3 dir =  curve.P2.GetGlobal () - curve.P3.GetGlobal ();

			var trackSegment = UnityEngine.Object.Instantiate<TrackSegment4>( ScriptableSingleton<AssetManager>.Instance.getPrefab<TrackSegment4>(Prefabs.Straight));

            TrackUIHandle.instance.TrackRide.Track.addSegment (trackSegment, _stateData.Selected.GetComponent<ExtrudeNode> ().TrackSegmentModify.GetIndexOfSegment ()+1);
			trackSegment.Initialize ();


            var modify = trackSegment.gameObject.AddComponent<TrackSegmentModify> ();
            _stateData.Selected.GetComponent<ExtrudeNode> ().TrackSegmentModify.ConnectWithForwardSegment(modify);


            //_stateData.Selected.GetComponent<ExtrudeNode>().TrackSegmentModify.GetLastCurve.P3

            modify.CalculateStartBinormal (true);
                
            _stateData.Selected = modify.GetLastCurve.P3.gameObject.transform;

            //re-apply p2-p3 because that is lost when the segments are merged
            TrackNodeHelper.CalculateMatch (curve.P2, curve.P3.GetGlobal() + dir);

            stateMachine.ChangeState(new FreeDragWithSmoothing(_stateData));

			//stateMachine.ChangeState (new IdleState (_stateData));
		}


		public void Unload()
		{
			
		}
	}
}

