using System;
using UnityEngine;
using System.Collections.Generic;

namespace RollercoasterEdit
{
	public class TrackNodeCurve
	{
		public enum Grouping
		{
			Start,
			Middle,
			Both,
			End
		}

		private CubicBezier _cubicBezier;
		private TrackSegmentModify _segmentModify;

		public TrackCurveNode P0{ get; private set; }
		public TrackCurveNode P1{ get; private set; }
		public TrackCurveNode P2{ get; private set; }
		public TrackCurveNode P3{ get; private set; }
		public Grouping Group { get; private set; }

		public TrackNodeCurve (CubicBezier cubicBezier, TrackSegmentModify segmentModify,Grouping grouping)
		{
			this.Group = grouping;
			this._cubicBezier = cubicBezier;
			this._segmentModify = segmentModify;

			P0 = AddNode ( _segmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p0),TrackCurveNode.NodeType.PO,!(this.Group == Grouping.Start || this.Group == Grouping.Both));
			P1 = AddNode (_segmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p1),TrackCurveNode.NodeType.P1,true);
			P2 = AddNode (_segmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p2),TrackCurveNode.NodeType.P2,true);
			P3 = AddNode (_segmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p3),TrackCurveNode.NodeType.P3,true);

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

		private TrackCurveNode AddNode(Vector3 position, TrackCurveNode.NodeType type,bool IsActive)
		{
			
			GameObject node = UnityEngine.Object.Instantiate( Main.AssetBundleManager.NodeGo);//GameObject.CreatePrimitive(PrimitiveType.Sphere);
			node.layer = 900;
			node.transform.transform.position = position;
			node.name = "BezierNode";

			var n = node.AddComponent< TrackCurveNode>();
			n.gameObject.SetActive (IsActive);
			n.TrackSegmentModify = _segmentModify;
			n.Curve = _cubicBezier;
			n.NodePoint = type;
			n.TrackCurve = this;
			return n;
		}

	
	}
}

