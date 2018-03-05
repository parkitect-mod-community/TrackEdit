using UnityEngine;

namespace TrackEdit
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

        private readonly CubicBezier _cubicBezier;


        public TrackNodeCurve(CubicBezier cubicBezier, TrackSegmentModify segmentModify, Grouping grouping)
        {
            Group = grouping;
            _cubicBezier = cubicBezier;
            SegmentModify = segmentModify;


            P0 = AddTrackCurveNode(SegmentModify.TrackSegment.transform.TransformPoint(cubicBezier.p0),
                TrackNode.NodeType.P0);
            P1 = AddTrackCurveNode(SegmentModify.TrackSegment.transform.TransformPoint(cubicBezier.p1),
                TrackNode.NodeType.P1);
            P2 = AddTrackCurveNode(SegmentModify.TrackSegment.transform.TransformPoint(cubicBezier.p2),
                TrackNode.NodeType.P2);
            P3 = AddTrackCurveNode(SegmentModify.TrackSegment.transform.TransformPoint(cubicBezier.p3),
                TrackNode.NodeType.P3);
            if ((grouping == Grouping.End || grouping == Grouping.Both) && SegmentModify.GetNextSegment(true) == null)
                ExtrudeNode = AddExtrudeNode(SegmentModify.TrackSegment.transform.TransformPoint(cubicBezier.p3) +
                                             SegmentModify.TrackSegment.getTangentPoint(1f) * .3f);
        }

        public TrackSegmentModify SegmentModify { get; }

        public TrackNode P0 { get; }
        public TrackNode P1 { get; }
        public TrackNode P2 { get; }
        public TrackNode P3 { get; }
        private ExtrudeNode ExtrudeNode { get; set; }
        public Grouping Group { get; }

        public void Update()
        {
            if ((Group == Grouping.End || Group == Grouping.Both) && SegmentModify.GetNextSegment(true) != null &&
                ExtrudeNode != null)
                Object.Destroy(ExtrudeNode.gameObject);
            else if ((Group == Grouping.End || Group == Grouping.Both) && SegmentModify.GetNextSegment(true) == null &&
                     ExtrudeNode == null)
                ExtrudeNode = AddExtrudeNode(SegmentModify.TrackSegment.transform.TransformPoint(_cubicBezier.p3) +
                                             SegmentModify.TrackSegment.getTangentPoint(1f) * .3f);
        }

        public void Destroy()
        {
            if (P0 != null)
                Object.Destroy(P0.gameObject);
            if (P1 != null)
                Object.Destroy(P1.gameObject);
            if (P2 != null)
                Object.Destroy(P2.gameObject);
            if (P3 != null)
                Object.Destroy(P3.gameObject);
            if (ExtrudeNode != null)
                Object.Destroy(ExtrudeNode.gameObject);
        }

        public void ClearExtrudeNode()
        {
            if (ExtrudeNode != null)
                Object.Destroy(ExtrudeNode.gameObject);
        }


        private ExtrudeNode AddExtrudeNode(Vector3 position)
        {
            var nodeGameObject = Object.Instantiate(Main.AssetBundleManager.NodeGo);
            nodeGameObject.layer = LayerMasks.COASTER_TRACKS;
            nodeGameObject.transform.transform.position = position;
            nodeGameObject.name = "ExtrudeNode";

            var node = nodeGameObject.AddComponent<ExtrudeNode>();
            node.TrackSegmentModify = SegmentModify;
            node.Curve = _cubicBezier;
            node.gameObject.layer = LayerMasks.ID_COASTER_TRACKS;
            node.TrackCurve = this;
            return node;
        }

        private TrackNode AddTrackCurveNode(Vector3 position, TrackNode.NodeType type)
        {
            var nodeGameObject = Object.Instantiate(Main.AssetBundleManager.NodeGo);
            nodeGameObject.layer = LayerMasks.COASTER_TRACKS;
            nodeGameObject.transform.transform.position = position;
            nodeGameObject.name = "BezierNode";

            var trackNode = nodeGameObject.AddComponent<TrackNode>();

            if (type == TrackNode.NodeType.P3)
                trackNode.ActiveState = TrackNode.Activestate.AlwaysActive;
            if (type == TrackNode.NodeType.P0)
                trackNode.ActiveState = TrackNode.Activestate.NeverActive;

            if (type == TrackNode.NodeType.P3)
            {
                var nodeRotate = Object.Instantiate(Main.AssetBundleManager.NodeRotateGo);
                nodeRotate.name = "MainRotate";
                nodeRotate.transform.SetParent(nodeGameObject.transform);
                nodeRotate.transform.position = nodeGameObject.transform.position;

                //add a roation ring
                var mainRotate = nodeRotate.transform.Find("Rotate").gameObject;
                mainRotate.layer = LayerMasks.ID_COASTER_TRACKS;
                trackNode.Rotate = mainRotate.AddComponent<RotationNode>();
            }

            var previousSegment = SegmentModify.GetPreviousSegment(true);
            var nextSegment = SegmentModify.GetNextSegment(true);

            if (SegmentModify.TrackSegment is Station)
            {
                trackNode.ActiveState = TrackNode.Activestate.NeverActive;
            }
            else
            {
                if (nextSegment != null)
                {
                    if (nextSegment.TrackSegment is Station && type == TrackNode.NodeType.P3)
                        trackNode.ActiveState = TrackNode.Activestate.NeverActive;
                    if (nextSegment.TrackSegment is Station && type == TrackNode.NodeType.P2)
                        trackNode.ActiveState = TrackNode.Activestate.AlwaysActive;
                }

                if (previousSegment != null)
                    if (previousSegment.TrackSegment is Station && type == TrackNode.NodeType.P1)
                        trackNode.ActiveState = TrackNode.Activestate.AlwaysActive;
            }


            trackNode.SetActiveState(false);
            trackNode.TrackSegmentModify = SegmentModify;
            trackNode.Curve = _cubicBezier;
            trackNode.NodePoint = type;
            trackNode.gameObject.layer = LayerMasks.ID_COASTER_TRACKS;
            trackNode.TrackCurve = this;
            return trackNode;
        }
    }
}