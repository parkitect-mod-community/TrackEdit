using UnityEngine;

namespace TrackEdit.StateMachine
{
    public class FreeDragState : DraggableState
    {
        private readonly BuilderHeightMarker _heightMaker;
        private readonly BuilderHeightMarker _p3HeightMarker;
        private readonly SharedStateData _stateData;

        public FreeDragState(SharedStateData stateData) : base(stateData)
        {
            _stateData = stateData;
            _heightMaker = Object.Instantiate(ScriptableSingleton<AssetManager>.Instance.builderHeightMarkerGO);
            _heightMaker.attachedTo = stateData.Selected.transform;
            _heightMaker.heightChangeDelta = .1f;


            var trackNode = stateData.Selected.gameObject.GetComponent<TrackNode>();
            var previousSegment = trackNode.TrackSegmentModify.GetPreviousSegment(true);
            Transform p3Hook = null;

            if (trackNode.NodePoint == TrackNode.NodeType.P1)
                p3Hook = previousSegment.GetLastCurve.P3.transform;
            else if (trackNode.NodePoint == TrackNode.NodeType.P2) p3Hook = trackNode.TrackCurve.P3.transform;

            if (p3Hook != stateData.Selected.transform && p3Hook != null)
            {
                _p3HeightMarker = Object.Instantiate(ScriptableSingleton<AssetManager>.Instance.builderHeightMarkerGO);
                _p3HeightMarker.attachedTo = p3Hook;
                _p3HeightMarker.heightChangeDelta = .01f;
            }
        }

        public override void Update(FiniteStateMachine stateMachine)
        {
            base.Update(stateMachine);

            var trackNode = _stateData.Selected.gameObject.GetComponent<TrackNode>();


            //calculate the match for the node segment
            TrackNodeHelper.CalculateMatch(trackNode, DragPosition);


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
            Object.Destroy(_heightMaker.gameObject);
            if (_p3HeightMarker != null)
                Object.Destroy(_p3HeightMarker.gameObject);
        }
    }
}