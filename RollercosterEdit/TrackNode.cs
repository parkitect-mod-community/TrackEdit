using System;
using UnityEngine;

namespace RollercoasterEdit
{
	public class TrackNode : MonoBehaviour
	{
		public enum NodeType  {
			PO,
			P1,
			P2,
			P3
		};

		public NodeType NodePoint;
		public CubicBezier Curve;
		public TrackSegmentModify TrackSegmentModify ;
		public TrackNodeCurve TrackCurve;

		private Vector3 _previousPos = new Vector3(); 

		public TrackNode ()
		{
			//this.transform.localPosition = new Vector3 (0, .5f, 0);

		}

		void Start()
		{

		}

		public void Initialize()
		{
		}


		public void SetPoint(Vector3 point)
		{
			Vector3 p = TrackSegmentModify.TrackSegment.transform.InverseTransformPoint (point) ;

			switch (NodePoint) {
			case NodeType.PO:
				_previousPos = Curve.p0;
				Curve.p0 = p;
				break;
			case NodeType.P1:
				_previousPos = Curve.p1;
				Curve.p1 =p;
				break;
			case NodeType.P2:
				_previousPos = Curve.p2;
				Curve.p2 =p;
				break;
			case NodeType.P3:
				_previousPos = Curve.p3;
				Curve.p3 =p;
				break;
			}
			this.transform.position = point;
		}

		public Vector3 GetLocal()
		{
			return TrackSegmentModify.TrackSegment.transform.InverseTransformPoint (this.transform.position);
		}

		public Vector3 GetGlobal()
		{

			switch (NodePoint) {
			case NodeType.PO:
				
				return TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p0);
		
			case NodeType.P1:
				return TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p1);

			case NodeType.P2:
				return TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p2);

			case NodeType.P3:
				return TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p3);

			}
			return Vector3.zero;
		}

		public void RollBack()
		{
			if (_previousPos != Vector3.zero) {
				switch (NodePoint) {
				case NodeType.PO:
					Curve.p0 = _previousPos ;

					break;
				case NodeType.P1:
					Curve.p1 = _previousPos;
					break;
				case NodeType.P2:
					Curve.p2 = _previousPos ;
					break;
				case NodeType.P3:
					Curve.p3 = _previousPos ;
					break;
				}
			}
		}

		public void UpdatePosition()
		{
				switch (NodePoint) {
				case NodeType.PO:
					this.transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p0) ;
					break;
				case NodeType.P1:
					this.transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p1) ;
					break;
				case NodeType.P2:
					this.transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p2) ;
					break;
				case NodeType.P3:
					this.transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p3) ;
					break;
				}

	
		}


		/*public void NodeUpdate()
		{
			var nextSegment = TrackSegmentModify.GetNextSegment ();
			var previousSegment = TrackSegmentModify.GetPreviousSegment ();


			switch (NodePoint) {
			case NodeType.PO:


					break;
			case NodeType.P1:
				this.SetPoint (this.transform.position);

				if (previousSegment != null) {

					CalculateLenghtAndNormals ();


					float magnitude = Mathf.Abs((previousSegment.GetLastCurve.P2.GetGlobal () - previousSegment.GetLastCurve.P3.GetGlobal ()).magnitude);
					previousSegment.GetLastCurve.P2.SetPoint (previousSegment.GetLastCurve.P3.GetGlobal() + (TrackSegmentModify.TrackSegment.getTangentPoint(0f) *-1f* magnitude));
					previousSegment.Invalidate = true;
					CalculateLenghtAndNormals ();

					TrackSegmentModify.CalculateStartBinormal ();
					//TrackSegmentModify.TrackSegment.upgradeSavegameRecalculateBinormal (previousSegment.TrackSegment);


				}


				break;
			case NodeType.P2:

				this.SetPoint (this.transform.position);

				if (nextSegment != null) {

					CalculateLenghtAndNormals ();


					float magnitude = Mathf.Abs((nextSegment.GetFirstCurve.P0.GetGlobal () - nextSegment.GetFirstCurve.P1.GetGlobal ()).magnitude);
					nextSegment.GetFirstCurve.P1.SetPoint (nextSegment.GetFirstCurve.P0.GetGlobal () + (TrackSegmentModify.TrackSegment.getTangentPoint(1f) * magnitude));
					nextSegment.Invalidate = true;
					CalculateLenghtAndNormals ();

					nextSegment.CalculateStartBinormal ();

				}


					break;
			case NodeType.P3:

				if (TrackCurve.Group == TrackNodeCurve.Grouping.End || TrackCurve.Group == TrackNodeCurve.Grouping.Both ) {

					if (nextSegment != null) {

						var NextP1Offset = nextSegment.GetFirstCurve.P1.GetGlobal() -GetGlobal();
						var P2Offset = TrackCurve.P2.GetGlobal () - GetGlobal ();


						nextSegment.GetFirstCurve.P0.SetPoint (this.transform.position);
						nextSegment.GetFirstCurve.P1.SetPoint (this.transform.position+ NextP1Offset);
						TrackCurve.P2.SetPoint(this.transform.position+ P2Offset);

						nextSegment.Invalidate = true;
					}

				}
				this.SetPoint (this.transform.position);

			break;
			}

			TrackSegmentModify.Invalidate = true;
			Validate ();

		}*/

        public void CalculateLenghtAndNormals()
		{
			var nextSegment = TrackSegmentModify.GetNextSegment ();
			var previousSegment = TrackSegmentModify.GetPreviousSegment ();


			if(previousSegment != null)
				previousSegment.TrackSegment.calculateLengthAndNormals (TrackSegmentModify.TrackSegment);

			if(nextSegment != null)
				TrackSegmentModify.TrackSegment.calculateLengthAndNormals (nextSegment.TrackSegment);

			if(nextSegment != null)
				nextSegment.TrackSegment.calculateLengthAndNormals (TrackSegmentModify.TrackSegment);
		}

		public bool Validate()
		{
			bool isValid = true;
			var nextSegment = TrackSegmentModify.GetNextSegment ();
			var previousSegment = TrackSegmentModify.GetPreviousSegment ();

			CalculateLenghtAndNormals ();

			if (previousSegment != null) {
				if (!previousSegment.TrackSegment.isConnectedTo (TrackSegmentModify.TrackSegment)) {
					previousSegment.RollBackSegment ();
					TrackSegmentModify.RollBackSegment ();
					isValid = false;
				}
			}
			if (nextSegment != null) {
				if (!TrackSegmentModify.TrackSegment.isConnectedTo (nextSegment.TrackSegment)) {
					TrackSegmentModify.RollBackSegment ();
					nextSegment.RollBackSegment ();
					isValid = false;

				}
			}

			CalculateLenghtAndNormals ();
			return isValid;

		}


	}
}

