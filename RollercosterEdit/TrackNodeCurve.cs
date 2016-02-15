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
		public ExtrudeNode extrudeNode{ get; private set; }
		public Grouping Group { get; private set; }

	

		public TrackNodeCurve (CubicBezier cubicBezier, TrackSegmentModify segmentModify,Grouping grouping)
		{
			this.Group = grouping;
			this._cubicBezier = cubicBezier;
			this.SegmentModify = segmentModify;


			P0 = AddTrackCurveNode ( SegmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p0),TrackNode.NodeType.PO );
			P1 = AddTrackCurveNode (SegmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p1), TrackNode.NodeType.P1);
			P2 = AddTrackCurveNode (SegmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p2),TrackNode.NodeType.P2);
			P3 = AddTrackCurveNode (SegmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p3),TrackNode.NodeType.P3);
			if ((grouping == Grouping.End || grouping == Grouping.Both) && SegmentModify.GetNextSegment() == null) {
				extrudeNode = AddExtrudeNode (SegmentModify.TrackSegment.transform.TransformPoint (_cubicBezier.p3) + SegmentModify.TrackSegment.getTangentPoint(1f)*.3f);
			}
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
			if (extrudeNode != null)
				UnityEngine.Object.Destroy (extrudeNode.gameObject);

		}

		public void ClearExtrudeNode()
		{
			if (extrudeNode != null)
				UnityEngine.Object.Destroy (extrudeNode.gameObject);
		}

		private ExtrudeNode AddExtrudeNode(Vector3 position)
		{
			GameObject node = UnityEngine.Object.Instantiate( Main.AssetBundleManager.NodeGo);//GameObject.CreatePrimitive(PrimitiveType.Sphere);
			node.layer = LayerMasks.COASTER_TRACKS;
			node.transform.transform.position = position;
			node.name = "ExtrudeNode";

			var n = node.AddComponent<ExtrudeNode>();
			n.TrackSegmentModify = SegmentModify;
			n.Curve = _cubicBezier;
			n.gameObject.layer = LayerMasks.ID_COASTER_TRACKS;
			n.TrackCurve = this;
			return n;

		}

		private TrackNode AddTrackCurveNode(Vector3 position, TrackNode.NodeType type)
		{
			
			GameObject node = UnityEngine.Object.Instantiate( Main.AssetBundleManager.NodeGo);//GameObject.CreatePrimitive(PrimitiveType.Sphere);
			node.layer = LayerMasks.COASTER_TRACKS;
			node.transform.transform.position = position;
			node.name = "BezierNode";

			var n = node.AddComponent< TrackNode>();

			if (type == TrackNode.NodeType.P3)
				n.ActiveState = TrackNode.Activestate.AlwaysActive;
			if (type == TrackNode.NodeType.PO)
				n.ActiveState = TrackNode.Activestate.NeverActive;

			var previousSegment = SegmentModify.GetPreviousSegment ();
			var nextSegment = SegmentModify.GetNextSegment ();

			if (this.SegmentModify.TrackSegment is Station)
				n.ActiveState = TrackNode.Activestate.NeverActive;
			else {

				if (nextSegment != null) {
					if (nextSegment.TrackSegment is Station && type == TrackNode.NodeType.P3)
						n.ActiveState = TrackNode.Activestate.NeverActive;
					if (nextSegment.TrackSegment is Station && type == TrackNode.NodeType.P2)
						n.ActiveState = TrackNode.Activestate.AlwaysActive;
				}
				if (previousSegment != null) {
					if (previousSegment.TrackSegment is Station && type == TrackNode.NodeType.P1) {
						n.ActiveState = TrackNode.Activestate.AlwaysActive;
					}
				}
			}

			n.SetActiveState (false);
			n.TrackSegmentModify = SegmentModify;
			n.Curve = _cubicBezier;
			n.NodePoint = type;
			n.gameObject.layer = LayerMasks.ID_COASTER_TRACKS;
			n.TrackCurve = this;
			return n;
		}

	
	}
}

