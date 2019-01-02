/**
* Copyright 2019 Michael Pollind
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using UnityEngine;

namespace TrackEdit
{
    public class TrackNode : MonoBehaviour, INode
    {
        public enum Activestate
        {
            AlwaysActive,
            Default,
            NeverActive
        }

        public enum NodeType
        {
            P0,
            P1,
            P2,
            P3
        }

        private LineRenderer _lineSegment;

        public Activestate ActiveState = Activestate.Default;
        public CubicBezier Curve;
        public NodeType NodePoint;
        public RotationNode Rotate;
        public TrackNodeCurve TrackCurve;
        public TrackSegmentModify TrackSegmentModify;

        public void ActivateNeighbors(bool active)
        {
            var nextCurve = TrackSegmentModify.GetNextCurve(TrackCurve);
            var previousCurve = TrackSegmentModify.GetPreviousCurve(TrackCurve);
            if (NodePoint != NodeType.P3) SetActiveState(active);
            switch (NodePoint)
            {
                case NodeType.P0:
                    //P0 Node is never active
                    break;
                case NodeType.P1:
                    //TrackCurve.P0.gameObject.SetActive (active);
                    if (previousCurve != null)
                    {
                        previousCurve.P2.SetActiveState(active);
                        previousCurve.P3.SetActiveState(active);
                    }

                    break;
                case NodeType.P2:
                    TrackCurve.P3.SetActiveState(active);
                    if (nextCurve != null) nextCurve.P1.SetActiveState(active);
                    break;
                case NodeType.P3:
                    TrackCurve.P2.SetActiveState(active);
                    if (nextCurve != null) nextCurve.P1.SetActiveState(active);
                    break;
            }

            if (Rotate != null)
                Rotate.transform.parent.gameObject.SetActive(active);
        }

        private void Start()
        {
            if (NodePoint == NodeType.P3)
                _lineSegment = gameObject.transform.Find("item").gameObject.GetComponent<LineRenderer>();
        }

        public void Initialize()
        {
        }

        private void Update()
        {
            if (NodePoint == NodeType.P3)
            {
                var nextCurve = TrackSegmentModify.GetNextCurve(TrackCurve);
                if (nextCurve != null)
                {
                    var v1 = transform.Find("item").position;
                    var v2 = transform.Find("item").position;
                    var v3 = transform.Find("item").position;

                    if (nextCurve.P1.isActiveAndEnabled) v1 = nextCurve.P1.transform.Find("item").position;
                    if (TrackCurve.P2.isActiveAndEnabled) v3 = TrackCurve.P2.transform.Find("item").position;


                    _lineSegment.SetPositions(new[]
                    {
                        v1,
                        v2,
                        v3
                    });
                }
            }

            //error checking to mark bad nodes
            var next = TrackSegmentModify.GetNextSegment(true);
            if (next != null && !TrackSegmentModify.TrackSegment.isConnectedTo(next.TrackSegment))
                transform.Find("item").GetComponent<Renderer>().material.color = new Color(1, 0, 0, .5f);
            else
                transform.Find("item").GetComponent<Renderer>().material.color = new Color(1, 1, 1, .5f);

            transform.Find("item").LookAt(Camera.main.transform, Vector3.down);
        }

        public void SetActiveState(bool active)
        {
            if (ActiveState == Activestate.AlwaysActive)
                gameObject.SetActive(true);
            else if (ActiveState == Activestate.NeverActive)
                gameObject.SetActive(false);
            else if (ActiveState == Activestate.Default) gameObject.SetActive(active);
            if (Rotate != null)
                Rotate.transform.parent.gameObject.SetActive(active);
        }

        public void SetPoint(Vector3 point)
        {
            var p = TrackSegmentModify.TrackSegment.transform.InverseTransformPoint(point);

            switch (NodePoint)
            {
                case NodeType.P0:
                    Curve.p0 = p;
                    break;
                case NodeType.P1:
                    Curve.p1 = p;
                    break;
                case NodeType.P2:
                    Curve.p2 = p;
                    break;
                case NodeType.P3:
                    Curve.p3 = p;
                    break;
            }

            transform.position = point;
        }

        public Vector3 GetLocal()
        {
            return TrackSegmentModify.TrackSegment.transform.InverseTransformPoint(transform.position);
        }

        public Vector3 GetGlobal()
        {
            switch (NodePoint)
            {
                case NodeType.P0:

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

        public void UpdatePosition()
        {
            switch (NodePoint)
            {
                case NodeType.P0:
                    transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p0);
                    break;
                case NodeType.P1:
                    transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p1);
                    break;
                case NodeType.P2:
                    transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p2);
                    break;
                case NodeType.P3:
                    transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p3);
                    break;
            }
        }


        public void CalculateLenghtAndNormals()
        {
            var nextSegment = TrackSegmentModify.GetNextSegment(true);
            var previousSegment = TrackSegmentModify.GetPreviousSegment(true);

            if (previousSegment != null)
                previousSegment.TrackSegment.calculateLengthAndNormals(TrackSegmentModify.TrackSegment);

            if (nextSegment != null)
                TrackSegmentModify.TrackSegment.calculateLengthAndNormals(nextSegment.TrackSegment);

            if (nextSegment != null)
                nextSegment.TrackSegment.calculateLengthAndNormals(TrackSegmentModify.TrackSegment);
        }
    }
}