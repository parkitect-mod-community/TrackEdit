using UnityEngine;

namespace TrackEdit.StateMachine
{
    public class FreeDragWithSmoothing : DraggableState
    {
        private readonly BuilderHeightMarker _heightMarker;
        private readonly SharedStateData _stateData;

        public FreeDragWithSmoothing(SharedStateData stateData) : base(stateData)
        {
            _stateData = stateData;

            _heightMarker = Object.Instantiate(ScriptableSingleton<AssetManager>.Instance.builderHeightMarkerGO);
            _heightMarker.attachedTo = stateData.Selected.transform;
            _heightMarker.heightChangeDelta = .01f;
        }

        public override void Update(FiniteStateMachine stateMachine)
        {
            var trackNode = _stateData.Selected.gameObject.GetComponent<TrackNode>();
            base.Update(stateMachine);

            //calculate the match for the node segment
            TrackNodeHelper.CalculateMatch(trackNode, DragPosition);
            var previousCurve = trackNode.TrackSegmentModify.GetPreviousCurve(trackNode.TrackCurve);

            var dist = Vector3.Distance(previousCurve.P3.GetGlobal(), trackNode.TrackCurve.P3.GetGlobal());
            var dir = (trackNode.TrackCurve.P3.GetGlobal() - previousCurve.P3.GetGlobal()).normalized;

            TrackNodeHelper.CalculateMatch(trackNode.TrackCurve.P1,
                trackNode.TrackCurve.P0.GetGlobal() + dist / 2.0f *
                (trackNode.TrackCurve.P1.GetGlobal() - trackNode.TrackCurve.P0.GetGlobal()).normalized);


            var p0 = trackNode.TrackCurve.P1.GetGlobal() - trackNode.TrackCurve.P0.GetGlobal();

            var normal = Vector3.Cross(dir, p0).normalized;
            var p1Final = -(Quaternion.AngleAxis(Vector3.Angle(-dir, p0), normal) * -dir).normalized;

            TrackNodeHelper.CalculateMatch(trackNode.TrackCurve.P2,
                trackNode.TrackCurve.P3.GetGlobal() + dist / 2.0f * p1Final);

            var nextSegment = trackNode.TrackSegmentModify.GetNextSegment(false);
            if (!_stateData.Selected.gameObject.GetComponent<TrackNode>().TrackSegmentModify.TrackSegment
                .isConnectedToNextSegment)
                if (_stateData.Selected.gameObject.GetComponent<TrackNode>().NodePoint == TrackNode.NodeType.P3 &&
                    (DragPosition - nextSegment.GetFirstCurve.P0.GetGlobal()).sqrMagnitude < .2f)
                {
                    var magnitude =
                        Mathf.Abs((nextSegment.GetFirstCurve.P0.GetGlobal() - nextSegment.GetFirstCurve.P1.GetGlobal())
                            .magnitude);


                    _stateData.Selected.gameObject.GetComponent<TrackNode>()
                        .SetPoint(nextSegment.GetFirstCurve.P0.GetGlobal());
                    _stateData.Selected.gameObject.GetComponent<TrackNode>().TrackSegmentModify.GetLastCurve.P2
                        .SetPoint(_stateData.Selected.gameObject.GetComponent<TrackNode>().TrackSegmentModify
                                      .GetLastCurve.P3.GetGlobal() +
                                  nextSegment.TrackSegment.getTangentPoint(0f) * -1f * magnitude);
                    _stateData.Selected.gameObject.GetComponent<TrackNode>().CalculateLenghtAndNormals();
                    _stateData.Selected.gameObject.GetComponent<TrackNode>().TrackSegmentModify.GetFirstCurve.P0
                        .TrackSegmentModify.CalculateStartBinormal(false);
                    _stateData.Selected.gameObject.GetComponent<TrackNode>().TrackSegmentModify.GetLastCurve
                        .ClearExtrudeNode();


                    nextSegment.GetFirstCurve.P0.CalculateLenghtAndNormals();
                    if (Input.GetMouseButtonUp(0))
                    {
                        _stateData.Selected.gameObject.GetComponent<TrackNode>().TrackSegmentModify
                            .ConnectWithForwardSegment(nextSegment);
                        stateMachine.ChangeState(new IdleState(_stateData));
                    }

                    nextSegment.Invalidate = true;

                    _stateData.Selected.gameObject.GetComponent<TrackNode>().TrackSegmentModify.Invalidate = true;
                }

            if (Input.GetMouseButtonUp(0)) stateMachine.ChangeState(new IdleState(_stateData));
        }

        public override void Unload()
        {
            base.Unload();
            Object.Destroy(_heightMarker.gameObject);
        }
    }
}