using System;
using RollercoasterEdit;
using UnityEngine;

namespace RollercoasterEdit
{
	public class LinearDragState  : IState
	{
		private SharedStateData _stateData;
		public LinearDragState (SharedStateData stateData)
		{
			this._stateData = stateData;
		}

		public void Update(FiniteStateMachine stateMachine)
		{
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			Vector3 point = ray.GetPoint (_stateData.Distance);
			Vector3 position = Vector3.zero;

			position = new Vector3 (point.x, _stateData.FixedY, point.z) + new Vector3(_stateData.Offset.x, _stateData.Offset.y, _stateData.Offset.z);


			TrackNode trackNode = _stateData.Selected.gameObject.GetComponent<TrackNode> ();
			var nextSegment = trackNode.TrackSegmentModify.GetNextSegment ();
			var previousSegment = trackNode.TrackSegmentModify.GetPreviousSegment ();

			switch (trackNode.NodePoint) {

			case TrackNode.NodeType.P1:
				
				{

					trackNode.CalculateLenghtAndNormals ();


					float magnitude = Mathf.Abs((position - previousSegment.GetLastCurve.P3.GetGlobal ()).magnitude);
					trackNode.SetPoint (previousSegment.GetLastCurve.P3.GetGlobal() + (previousSegment.TrackSegment.getTangentPoint(1f) * magnitude));

					previousSegment.Invalidate = true;
					trackNode.CalculateLenghtAndNormals ();

					trackNode.TrackSegmentModify.CalculateStartBinormal ();
					//TrackSegmentModify.TrackSegment.upgradeSavegameRecalculateBinormal (previousSegment.TrackSegment);


				}


				break;
			case TrackNode.NodeType.P2:


				trackNode.CalculateLenghtAndNormals ();

				{

					float magnitude = Mathf.Abs ((nextSegment.GetFirstCurve.P0.GetGlobal () - position).magnitude);

					trackNode.SetPoint (nextSegment.GetFirstCurve.P0.GetGlobal () + (nextSegment.TrackSegment.getTangentPoint (0.0f) * -1.0f * magnitude));

					nextSegment.Invalidate = true;
					trackNode.CalculateLenghtAndNormals ();

					nextSegment.CalculateStartBinormal ();
				}

				break;
			}

			//  _stateData.Selected.position = position;

			trackNode.Validate ();
			trackNode.TrackSegmentModify.Invalidate = true;


			if (Input.GetMouseButtonUp (0)) {
				stateMachine.ChangeState(new IdleState (_stateData.SegmentManager));
			}
		}
		public void Unload()
		{
		}
	}
}

