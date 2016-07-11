using System;
using RollercoasterEdit;
using UnityEngine;

namespace RollercoasterEdit
{
	public class LinearDragState  : IState
	{
        private SharedStateData stateData;
		public LinearDragState (SharedStateData stateData)
		{
			this.stateData = stateData;
		}

		public void Update(FiniteStateMachine stateMachine)
		{
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			Vector3 point = ray.GetPoint (stateData.Distance);
			Vector3 position = Vector3.zero;

			position = new Vector3 (point.x, stateData.FixedY, point.z) + new Vector3(stateData.Offset.x, stateData.Offset.y, stateData.Offset.z);


			TrackNode trackNode = stateData.Selected.gameObject.GetComponent<TrackNode> ();
			var nextSegment = trackNode.trackSegmentModify.GetNextSegment (true);
			var previousSegment = trackNode.trackSegmentModify.GetPreviousSegment (true);

			switch (trackNode.nodePoint) {

			case TrackNode.NodeType.P1:
				
				{

					trackNode.CalculateLenghtAndNormals ();


					float magnitude = Mathf.Abs((position - previousSegment.GetLastCurve.P3.GetGlobal ()).magnitude);
					trackNode.SetPoint (previousSegment.GetLastCurve.P3.GetGlobal() + (previousSegment.TrackSegment.getTangentPoint(1f) * magnitude));

					previousSegment.invalidate = true;
					trackNode.CalculateLenghtAndNormals ();

					trackNode.trackSegmentModify.CalculateStartBinormal (true);
					//TrackSegmentModify.TrackSegment.upgradeSavegameRecalculateBinormal (previousSegment.TrackSegment);


				}


				break;
			case TrackNode.NodeType.P2:


				trackNode.CalculateLenghtAndNormals ();

				{

					float magnitude = Mathf.Abs ((nextSegment.GetFirstCurve.P0.GetGlobal () - position).magnitude);

					trackNode.SetPoint (nextSegment.GetFirstCurve.P0.GetGlobal () + (nextSegment.TrackSegment.getTangentPoint (0.0f) * -1.0f * magnitude));

					nextSegment.invalidate = true;
					trackNode.CalculateLenghtAndNormals ();

					nextSegment.CalculateStartBinormal (true);
				}

				break;
			}

            trackNode.CalculateLenghtAndNormals ();
			trackNode.trackSegmentModify.invalidate = true;


			if (Input.GetMouseButtonUp (0)) {
				stateMachine.ChangeState(new IdleState (stateData));
			}
		}
		public void Unload()
		{
		}
	}
}

