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

		public void NodeUpdate()
		{

			Vector3 p = TrackSegmentModify.TrackSegment.transform.InverseTransformPoint (this.transform.position) -offset;
			switch (NodePoint) {
			case NodeType.PO:
				Curve.p0 =	p;
					break;
			case NodeType.P1:
				Curve.p1 =p;
					break;
			case NodeType.P2:
				Curve.p2 = p;
					break;
			case NodeType.P3:
				//Vector3 previsionPosition = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p3);
				Curve.p3 = p;

				/*if (TrackSegmentModify.GetNextSegment () != null) {
				
				}*/
				/*if (Segment.isConnectedToNextSegment) {
					var nextSegement = PreciseModify.TrackRide.Track.trackSegments [PreciseModify.TrackRide.Track.trackSegments.IndexOf (Segment) + 1];
					nextSegement.curves [0].p0 = nextSegement.transform.InverseTransformPoint (this.transform.position) - offset;

				}*/
			break;
			}
			TrackSegmentModify.Invalidate = true;

		}


	}
}

