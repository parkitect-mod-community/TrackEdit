using UnityEngine;

namespace TrackEdit.StateMachine
{
    public class LinearDragState : IState
    {
        private readonly SharedStateData _stateData;

        public LinearDragState(SharedStateData stateData)
        {
            _stateData = stateData;
        }

        public void Update(FiniteStateMachine stateMachine)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var point = ray.GetPoint(_stateData.Distance);
            Vector3 position;

            position = new Vector3(point.x, _stateData.FixedY, point.z) +
                       new Vector3(_stateData.Offset.x, _stateData.Offset.y, _stateData.Offset.z);


            var trackNode = _stateData.Selected.gameObject.GetComponent<TrackNode>();
            var nextSegment = trackNode.TrackSegmentModify.GetNextSegment(true);
            var previousSegment = trackNode.TrackSegmentModify.GetPreviousSegment(true);

            switch (trackNode.NodePoint)
            {
                case TrackNode.NodeType.P1:

                {
                    trackNode.CalculateLenghtAndNormals();


                    var magnitude = Mathf.Abs((position - previousSegment.GetLastCurve.P3.GetGlobal()).magnitude);
                    trackNode.SetPoint(previousSegment.GetLastCurve.P3.GetGlobal() +
                                       previousSegment.TrackSegment.getTangentPoint(1f) * magnitude);

                    previousSegment.Invalidate = true;
                    trackNode.CalculateLenghtAndNormals();

                    trackNode.TrackSegmentModify.CalculateStartBinormal(true);
                    //TrackSegmentModify.TrackSegment.upgradeSavegameRecalculateBinormal (previousSegment.TrackSegment);
                }


                    break;
                case TrackNode.NodeType.P2:


                    trackNode.CalculateLenghtAndNormals();

                {
                    var magnitude = Mathf.Abs((nextSegment.GetFirstCurve.P0.GetGlobal() - position).magnitude);

                    trackNode.SetPoint(nextSegment.GetFirstCurve.P0.GetGlobal() +
                                       nextSegment.TrackSegment.getTangentPoint(0.0f) * -1.0f * magnitude);

                    nextSegment.Invalidate = true;
                    trackNode.CalculateLenghtAndNormals();

                    nextSegment.CalculateStartBinormal(true);
                }

                    break;
            }

            trackNode.CalculateLenghtAndNormals();
            trackNode.TrackSegmentModify.Invalidate = true;


            if (Input.GetMouseButtonUp(0)) stateMachine.ChangeState(new IdleState(_stateData));
        }

        public void Unload()
        {
        }
    }
}