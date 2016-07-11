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

        private CubicBezier cubicBezier;
		public TrackSegmentModify SegmentModify{ get; private set; }

		public TrackNode P0{ get; private set; }
		public TrackNode P1{ get; private set; }
		public TrackNode P2{ get; private set; }
		public TrackNode P3{ get; private set; }
		public ExtrudeNode ExtrudeNode{ get; private set; }
		public Grouping Group { get; private set; }
        public RotationNode Rotation { get; private set; }
	

		public TrackNodeCurve (CubicBezier cubicBezier, TrackSegmentModify segmentModify,Grouping grouping)
		{
			this.Group = grouping;
			this.cubicBezier = cubicBezier;
			this.SegmentModify = segmentModify;


			P0 = AddTrackCurveNode ( SegmentModify.TrackSegment.transform.TransformPoint (cubicBezier.p0),TrackNode.NodeType.PO );
			P1 = AddTrackCurveNode (SegmentModify.TrackSegment.transform.TransformPoint (cubicBezier.p1), TrackNode.NodeType.P1);
			P2 = AddTrackCurveNode (SegmentModify.TrackSegment.transform.TransformPoint (cubicBezier.p2),TrackNode.NodeType.P2);
			P3 = AddTrackCurveNode (SegmentModify.TrackSegment.transform.TransformPoint (cubicBezier.p3),TrackNode.NodeType.P3);
			if ((grouping == Grouping.End || grouping == Grouping.Both) && SegmentModify.GetNextSegment(true) == null) {
				ExtrudeNode = AddExtrudeNode (SegmentModify.TrackSegment.transform.TransformPoint (cubicBezier.p3) + SegmentModify.TrackSegment.getTangentPoint(1f)*.3f);
			}
		}

        public void Update()
        {
            if ((this.Group == Grouping.End || this.Group == Grouping.Both) && SegmentModify.GetNextSegment (true) != null && ExtrudeNode != null) {
                UnityEngine.GameObject.Destroy (ExtrudeNode.gameObject);
            } else if ((this.Group == Grouping.End || this.Group == Grouping.Both) && SegmentModify.GetNextSegment (true) == null && ExtrudeNode == null) {
                ExtrudeNode = AddExtrudeNode (SegmentModify.TrackSegment.transform.TransformPoint (cubicBezier.p3) + SegmentModify.TrackSegment.getTangentPoint(1f)*.3f);
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
			if (ExtrudeNode != null)
				UnityEngine.Object.Destroy (ExtrudeNode.gameObject);

		}

		public void ClearExtrudeNode()
		{
			if (ExtrudeNode != null)
				UnityEngine.Object.Destroy (ExtrudeNode.gameObject);
		}


		private ExtrudeNode AddExtrudeNode(Vector3 position)
		{
            GameObject nodeGameObject = UnityEngine.Object.Instantiate( Main.AssetBundleManager.nodeGo);
			nodeGameObject.layer = LayerMasks.COASTER_TRACKS;
			nodeGameObject.transform.transform.position = position;
			nodeGameObject.name = "ExtrudeNode";

            var node = nodeGameObject.AddComponent<ExtrudeNode>();
			node.trackSegmentModify = SegmentModify;
			node.curve = cubicBezier;
			node.gameObject.layer = LayerMasks.ID_COASTER_TRACKS;
			node.trackCurve = this;
			return node;

		}

		private TrackNode AddTrackCurveNode(Vector3 position, TrackNode.NodeType type)
		{
			
            GameObject nodeGameObject = UnityEngine.Object.Instantiate( Main.AssetBundleManager.nodeGo);
			nodeGameObject.layer = LayerMasks.COASTER_TRACKS;
			nodeGameObject.transform.transform.position = position;
			nodeGameObject.name = "BezierNode";

            var trackNode = nodeGameObject.AddComponent< TrackNode>();

			if (type == TrackNode.NodeType.P3)
				trackNode.activeState = TrackNode.Activestate.AlwaysActive;
			if (type == TrackNode.NodeType.PO)
				trackNode.activeState = TrackNode.Activestate.NeverActive;

            if (type == TrackNode.NodeType.P3) {
                GameObject nodeRotate = UnityEngine.Object.Instantiate (Main.AssetBundleManager.nodeRotateGo);
                nodeRotate.name = "MainRotate";
                nodeRotate.transform.SetParent (nodeGameObject.transform);
                nodeRotate.transform.position = nodeGameObject.transform.position;

                //add a roation ring
                GameObject mainRotate = nodeRotate.transform.Find ("Rotate").gameObject;
                mainRotate.layer = LayerMasks.ID_COASTER_TRACKS;
                trackNode.rotate =  mainRotate.AddComponent<RotationNode> ();

            }

			var previousSegment = SegmentModify.GetPreviousSegment (true);
			var nextSegment = SegmentModify.GetNextSegment (true);

			if (this.SegmentModify.TrackSegment is Station)
				trackNode.activeState = TrackNode.Activestate.NeverActive;
			else {

				if (nextSegment != null) {
					if (nextSegment.TrackSegment is Station && type == TrackNode.NodeType.P3)
						trackNode.activeState = TrackNode.Activestate.NeverActive;
					if (nextSegment.TrackSegment is Station && type == TrackNode.NodeType.P2)
						trackNode.activeState = TrackNode.Activestate.AlwaysActive;
				}
				if (previousSegment != null) {
					if (previousSegment.TrackSegment is Station && type == TrackNode.NodeType.P1) {
						trackNode.activeState = TrackNode.Activestate.AlwaysActive;
					}
				}
			}
           

			trackNode.SetActiveState (false);
			trackNode.trackSegmentModify = SegmentModify;
			trackNode.curve = cubicBezier;
			trackNode.nodePoint = type;
			trackNode.gameObject.layer = LayerMasks.ID_COASTER_TRACKS;
			trackNode.trackCurve = this;
			return trackNode;
		}

	
	}
}

