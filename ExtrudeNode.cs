using UnityEngine;

namespace TrackEdit
{
    public class ExtrudeNode : MonoBehaviour, INode
    {
        public CubicBezier Curve;
        public TrackNodeCurve TrackCurve;
        public TrackSegmentModify TrackSegmentModify;

        private void Update()
        {
            transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p3) +
                                 TrackSegmentModify.TrackSegment.getTangentPoint(1f) * .3f;

            transform.Find("item").GetComponent<Renderer>().material.color = new Color(0, 1, 0, .5f);
            transform.Find("item").LookAt(Camera.main.transform, Vector3.down);
        }
    }
}