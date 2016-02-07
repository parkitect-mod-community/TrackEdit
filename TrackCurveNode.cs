using System;
using UnityEngine;

namespace HelloMod
{
	public class TrackCurveNode : MonoBehaviour
	{
		private readonly Vector3 offset = new Vector3 (0, .5f, 0);


		public enum NodeType  {
			PO,
			P1,
			P2,
			P3
		};

		public NodeType NodePoint;
		public CubicBezier Curve;
		public TrackSegmentModify TrackSegmentModify;
		public TrackNodeCurve TrackCurve;

		private Vector3 _previousPos = new Vector3(); 

		public TrackCurveNode ()
		{
			this.transform.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
		}

		void Start()
		{
			this.transform.position += offset;

		}

		public void Initialize()
		{
		}


		public void SetPoint(Vector3 point)
		{
			Vector3 p = TrackSegmentModify.TrackSegment.transform.InverseTransformPoint (point) - offset;

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
				
				return TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p0)+offset;
		
			case NodeType.P1:
				return TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p1)+offset;

			case NodeType.P2:
				return TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p2)+offset;

			case NodeType.P3:
				return TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p3)+offset;

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
			if (_previousPos != Vector3.zero) {
				switch (NodePoint) {
				case NodeType.PO:
					Curve.p0 = _previousPos;
					this.transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p0) + offset;
					break;
				case NodeType.P1:
					Curve.p1 = _previousPos;
					this.transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p1) + offset;
					break;
				case NodeType.P2:
					Curve.p2 = _previousPos ;
					this.transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p2) + offset;

					break;
				case NodeType.P3:
					Curve.p3 = _previousPos ;
					this.transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p3) + offset;
					break;
				}
				_previousPos = Vector3.zero;
			}
		}


		public void NodeUpdate()
		{
			var nextSegment = TrackSegmentModify.GetNextSegment ();
			var previousSegment = TrackSegmentModify.GetPreviousSegment ();


			switch (NodePoint) {
			case NodeType.PO:


					break;
			case NodeType.P1:
				if (previousSegment != null) {

					Vector3 directionP0P1 = (TrackCurve.P1.GetGlobal () - TrackCurve.P0.GetGlobal ()).normalized * -1;
					float magnitude = Mathf.Abs((previousSegment.GetLastCurve.P2.GetGlobal () - previousSegment.GetLastCurve.P3.GetGlobal ()).magnitude);


					previousSegment.GetLastCurve.P2.SetPoint (previousSegment.GetLastCurve.P3.GetGlobal() + (directionP0P1 * magnitude));


					previousSegment.Invalidate = true;
				}

				break;
			case NodeType.P2:

				if (nextSegment != null) {

					Vector3 directionP2P3 = (TrackCurve.P2.GetGlobal () - TrackCurve.P3.GetGlobal ()).normalized * -1;
					float magnitude = Mathf.Abs((nextSegment.GetFirstCurve.P0.GetGlobal () - nextSegment.GetFirstCurve.P1.GetGlobal ()).magnitude);


					nextSegment.GetFirstCurve.P1.SetPoint (nextSegment.GetFirstCurve.P0.GetGlobal() + (directionP2P3 * magnitude));


					nextSegment.Invalidate = true;
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
			break;
			}

			this.SetPoint (this.transform.position);

			if(previousSegment != null)
			previousSegment.TrackSegment.calculateLengthAndNormals (TrackSegmentModify.TrackSegment);
			TrackSegmentModify.TrackSegment.calculateLengthAndNormals (nextSegment.TrackSegment);
			if(nextSegment != null)
			nextSegment.TrackSegment.calculateLengthAndNormals (TrackSegmentModify.TrackSegment);


			if (previousSegment != null) {
				if (!previousSegment.TrackSegment.isConnectedTo (TrackSegmentModify.TrackSegment)) {
					previousSegment.RollBackSegment ();
					TrackSegmentModify.RollBackSegment ();

				}
			}
			if (nextSegment != null) {
				if (!TrackSegmentModify.TrackSegment.isConnectedTo (nextSegment.TrackSegment)) {
					TrackSegmentModify.RollBackSegment ();
					nextSegment.RollBackSegment ();
				}
			}

			if(previousSegment != null)
			previousSegment.TrackSegment.calculateLengthAndNormals (TrackSegmentModify.TrackSegment);
			TrackSegmentModify.TrackSegment.calculateLengthAndNormals (nextSegment.TrackSegment);
			if(nextSegment != null)
			nextSegment.TrackSegment.calculateLengthAndNormals (TrackSegmentModify.TrackSegment);

			TrackSegmentModify.Invalidate = true;

		}


	}
}

