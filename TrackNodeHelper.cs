using UnityEngine;

namespace TrackEdit
{
    public static class TrackNodeHelper
    {
        public static void CalculateMatch(TrackNode trackNode, Vector3 position)
        {
            var nextSegment = trackNode.TrackSegmentModify.GetNextSegment(true);
            var previousSegment = trackNode.TrackSegmentModify.GetPreviousSegment(true);

            switch (trackNode.NodePoint)
            {
                case TrackNode.NodeType.P0:
                    break;
                case TrackNode.NodeType.P1:
                    trackNode.SetPoint(position);

                    if (previousSegment != null)
                    {
                        var magnitude = Mathf.Abs((previousSegment.GetLastCurve.P2.GetGlobal() -
                                                   previousSegment.GetLastCurve.P3.GetGlobal()).magnitude);
                        previousSegment.GetLastCurve.P2.SetPoint(
                            previousSegment.GetLastCurve.P3.GetGlobal() +
                            trackNode.TrackSegmentModify.TrackSegment.getTangentPoint(0f) * -1f * magnitude);
                        previousSegment.Invalidate = true;

                        trackNode.CalculateLenghtAndNormals();
                        trackNode.TrackSegmentModify.CalculateStartBinormal(true);
                    }


                    break;
                case TrackNode.NodeType.P2:

                    trackNode.SetPoint(position);

                    if (nextSegment != null)
                    {
                        var magnitude =
                            Mathf.Abs((nextSegment.GetFirstCurve.P0.GetGlobal() -
                                       nextSegment.GetFirstCurve.P1.GetGlobal()).magnitude);
                        nextSegment.GetFirstCurve.P1.SetPoint(nextSegment.GetFirstCurve.P0.GetGlobal() +
                                                              trackNode.TrackSegmentModify.TrackSegment.getTangentPoint(
                                                                  1f) * magnitude);
                        nextSegment.Invalidate = true;

                        trackNode.CalculateLenghtAndNormals();
                        nextSegment.CalculateStartBinormal(true);
                    }


                    break;
                case TrackNode.NodeType.P3:

                    if (trackNode.TrackCurve.Group == TrackNodeCurve.Grouping.End ||
                        trackNode.TrackCurve.Group == TrackNodeCurve.Grouping.Both)
                    {
                        var p2Offset = trackNode.TrackCurve.P2.GetGlobal() - trackNode.GetGlobal();
                        trackNode.TrackCurve.P2.SetPoint(position + p2Offset);

                        if (nextSegment != null)
                        {
                            var nextP1Offset = nextSegment.GetFirstCurve.P1.GetGlobal() - trackNode.GetGlobal();

                            nextSegment.GetFirstCurve.P0.SetPoint(position);
                            nextSegment.GetFirstCurve.P1.SetPoint(position + nextP1Offset);

                            nextSegment.Invalidate = true;
                        }
                    }

                    trackNode.SetPoint(position);

                    break;
            }

            trackNode.CalculateLenghtAndNormals();
            trackNode.TrackSegmentModify.Invalidate = true;
        }
    }
}