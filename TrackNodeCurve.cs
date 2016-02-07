using System;
using UnityEngine;
using System.Collections.Generic;

namespace HelloMod
{
	public class TrackNodeCurve
	{
		private CubicBezier _cubicBezier;
		private TrackSegmentModify _segmentModify;

		public TrackCurveNode P0;
		public TrackCurveNode P1;
		public TrackCurveNode P2;
		public TrackCurveNode P3;

		public TrackNodeCurve (CubicBezier cubicBezier, TrackSegmentModify segmentModify,bool beginning)
		{
			this._cubicBezier = cubicBezier;
			this._segmentModify = segmentModify;

			if(!beginning)
				P0 = AddNode ( _segmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p0),TrackCurveNode.NodeType.PO);
			
			P1 = AddNode (_segmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p1),TrackCurveNode.NodeType.P1);
			P2 = AddNode (_segmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p2),TrackCurveNode.NodeType.P2);
			P3 = AddNode (_segmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p3),TrackCurveNode.NodeType.P3);

		}

		public void Destroy()
		{
			if(P0 != null)
			UnityEngine.Object.Destroy (P0.gameObject);
			if(P1 != null)
			UnityEngine.Object.Destroy (P1.gameObject);
			if(P2 != null)
			UnityEngine.Object.Destroy (P2.gameObject);
			if(P3 != null)
			UnityEngine.Object.Destroy (P3.gameObject);

		}

		private TrackCurveNode AddNode(Vector3 position, TrackCurveNode.NodeType type)
		{
			GameObject node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			node.transform.name = "BezierNode";
			node.layer = 900;
			node.transform.transform.position = position;
			var n = node.AddComponent< TrackCurveNode>();
			n.TrackSegmentModify = _segmentModify;
			n.Curve = _cubicBezier;
			n.NodePoint = type;
			return n;
		}

	
	}
}

