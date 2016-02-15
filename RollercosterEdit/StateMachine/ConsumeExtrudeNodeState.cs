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
			_stateData.SegmentManager.TrackRide.Track.addSegment (trackSegment, _stateData.Selected.GetComponent<ExtrudeNode> ().TrackSegmentModify.GetIndexOfSegment ()+1);
			trackSegment.Initialize ();
			TrackSegmentManager.TrackSegmentManagerRefresh refresh = null; 
			refresh = new TrackSegmentManager.TrackSegmentManagerRefresh (delegate() {

				var segment = _stateData.SegmentManager.GetTrackSegmentModifyer(trackSegment);
				_stateData.SegmentManager.ConnectEndPieces(segment.GetPreviousSegment(),segment);

				_stateData.Selected = segment.GetLastCurve.P3.gameObject.transform;
				stateMachine.ChangeState(new FreeDragState(_stateData));

				_stateData.SegmentManager.OnRefresh -= refresh;
			});
			_stateData.SegmentManager.OnRefresh += refresh;

			stateMachine.ChangeState (new IdleState (_stateData));
		}


		public void Unload()
		{
			
		}
	}
}

