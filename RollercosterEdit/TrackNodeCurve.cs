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
		public TrackSegmentModify SegmentModify{ get; private set; }

		public TrackNode P0{ get; private set; }
		public TrackNode P1{ get; private set; }
		public TrackNode P2{ get; private set; }
		public TrackNode P3{ get; private set; }
		public Grouping Group { get; private set; }

		public TrackNodeCurve (CubicBezier cubicBezier, TrackSegmentModify segmentModify,Grouping grouping)
		{
			this.Group = grouping;
			this._cubicBezier = cubicBezier;
			this.SegmentModify = segmentModify;

			bool isEnable = true;

			if (segmentModify.TrackSegment is Station) {
				isEnable = false;
			}

			P0 = AddNode ( SegmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p0),TrackNode.NodeType.PO,isEnable && !(this.Group == Grouping.Start || this.Group == Grouping.Both));
			P1 = AddNode (SegmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p1),TrackNode.NodeType.P1,isEnable && true);
			P2 = AddNode (SegmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p2),TrackNode.NodeType.P2,isEnable && true);

			if (segmentModify.GetNextSegment()  != null && segmentModify.GetNextSegment().TrackSegment is Station) {
				isEnable = false;
			}
			P3 = AddNode (SegmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p3),TrackNode.NodeType.P3,isEnable && true);

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

		private TrackNode AddNode(Vector3 position, TrackNode.NodeType type,bool IsActive)
		{
			
			GameObject node = UnityEngine.Object.Instantiate( Main.AssetBundleManager.NodeGo);//GameObject.CreatePrimitive(PrimitiveType.Sphere);
			node.layer = LayerMasks.COASTER_TRACKS;
			node.transform.transform.position = position;
			node.name = "BezierNode";

			var n = node.AddComponent< TrackNode>();
			n.gameObject.SetActive (IsActive);
			n.TrackSegmentModify = SegmentModify;
			n.Curve = _cubicBezier;
			n.NodePoint = type;
			n.gameObject.layer = LayerMasks.ID_COASTER_TRACKS;
			n.TrackCurve = this;
			return n;
		}

	
	}
}

