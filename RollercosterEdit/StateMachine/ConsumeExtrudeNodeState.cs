using System;

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
            
			var trackSegment = UnityEngine.Object.Instantiate<TrackSegment4>( ScriptableSingleton<AssetManager>.Instance.getPrefab<TrackSegment4>(Prefabs.Straight));

            TrackUIHandle.instance.TrackRide.Track.addSegment (trackSegment, _stateData.Selected.GetComponent<ExtrudeNode> ().TrackSegmentModify.GetIndexOfSegment ()+1);
			trackSegment.Initialize ();

            var modify = trackSegment.gameObject.AddComponent<TrackSegmentModify> ();
            _stateData.Selected.GetComponent<ExtrudeNode> ().TrackSegmentModify.ConnectWithForwardSegment(modify);

            _stateData.Selected = modify.GetLastCurve.P3.gameObject.transform;
			stateMachine.ChangeState(new FreeDragState(_stateData));

			//stateMachine.ChangeState (new IdleState (_stateData));
		}


		public void Unload()
		{
			
		}
	}
}

