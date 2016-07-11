using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class TrackNodeHelper
    {
        public static void CalculateMatch(TrackNode trackNode,Vector3 position)
        {
            var nextSegment = trackNode.trackSegmentModify.GetNextSegment (true);
            var previousSegment = trackNode.trackSegmentModify.GetPreviousSegment (true);

            switch (trackNode.nodePoint) {
            case TrackNode.NodeType.PO:
                break;
            case TrackNode.NodeType.P1:
                trackNode.SetPoint (position);

                if (previousSegment != null) {

                    float magnitude = Mathf.Abs((previousSegment.GetLastCurve.P2.GetGlobal () - previousSegment.GetLastCurve.P3.GetGlobal ()).magnitude);
                    previousSegment.GetLastCurve.P2.SetPoint (previousSegment.GetLastCurve.P3.GetGlobal() + (trackNode.trackSegmentModify.TrackSegment.getTangentPoint(0f) *-1f* magnitude));
                    previousSegment.invalidate = true;

                    trackNode.CalculateLenghtAndNormals ();
                    trackNode.trackSegmentModify.CalculateStartBinormal (true);
                }


                break;
            case TrackNode.NodeType.P2:

                trackNode.SetPoint (position);

                if (nextSegment != null) {
                    float magnitude = Mathf.Abs((nextSegment.GetFirstCurve.P0.GetGlobal () - nextSegment.GetFirstCurve.P1.GetGlobal ()).magnitude);
                    nextSegment.GetFirstCurve.P1.SetPoint (nextSegment.GetFirstCurve.P0.GetGlobal () + (trackNode.trackSegmentModify.TrackSegment.getTangentPoint(1f) * magnitude));
                    nextSegment.invalidate = true;

                    trackNode.CalculateLenghtAndNormals ();
                    nextSegment.CalculateStartBinormal (true);

                }


                break;
            case TrackNode.NodeType.P3:

                if (trackNode.trackCurve.Group == TrackNodeCurve.Grouping.End || trackNode.trackCurve.Group == TrackNodeCurve.Grouping.Both ) {

                    var P2Offset = trackNode.trackCurve.P2.GetGlobal () - trackNode.GetGlobal ();
                    trackNode.trackCurve.P2.SetPoint(position+ P2Offset);

                    if (nextSegment != null) {

                        var nextP1Offset = nextSegment.GetFirstCurve.P1.GetGlobal() -trackNode.GetGlobal();

                        nextSegment.GetFirstCurve.P0.SetPoint (position);
                        nextSegment.GetFirstCurve.P1.SetPoint (position+ nextP1Offset);

                        nextSegment.invalidate = true;
                    }

                }
                trackNode.SetPoint (position);

                break;
            }

            trackNode.CalculateLenghtAndNormals ();
            trackNode.trackSegmentModify.invalidate = true;
        }
        
    }
}

