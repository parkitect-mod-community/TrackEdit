using UnityEngine;

namespace TrackEdit.StateMachine
{
    public class RotationState : IState
    {
        private readonly SharedStateData _stateData;

        public RotationState(SharedStateData stateData)
        {
            _stateData = stateData;
        }

        public void Update(FiniteStateMachine stateMachine)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            var rotationNode = _stateData.Selected.GetComponent<RotationNode>();

            var planeNormal = rotationNode.AttachedNode.TrackSegmentModify.TrackSegment.getTangentPoint(1.0f);
            var planeCenter = rotationNode.transform.position;

            var l = planeNormal.x * ray.origin.x + planeNormal.y * ray.origin.y + planeNormal.z * ray.origin.z;
            var r = planeNormal.x * planeCenter.x + planeNormal.y * planeCenter.y + planeCenter.z * planeNormal.z;

            var t = (-l + r) / (planeNormal.x * ray.direction.x + planeNormal.y * ray.direction.y +
                                planeNormal.z * ray.direction.z);

            var loc = ray.origin + ray.direction * t;
            var diff = MathHelper.AngleSigned(rotationNode.AttachedNode.TrackSegmentModify.TrackSegment.getNormal(1.0f),
                Vector3.Normalize(planeCenter - loc), planeNormal);

            rotationNode.AttachedNode.TrackSegmentModify.CalculateWithNewTotalRotation(
                Mathf.Round(diff + rotationNode.AttachedNode.TrackSegmentModify.TrackSegment.totalRotation));
            rotationNode.AttachedNode.TrackSegmentModify.Invalidate = true;

            var nextSegment = rotationNode.AttachedNode.TrackSegmentModify.GetNextSegment(true);

            if (nextSegment != null)
                nextSegment.Invalidate = true;

            if (Input.GetMouseButtonUp(0)) stateMachine.ChangeState(new IdleState(_stateData));
        }


        public void Unload()
        {
        }
    }
}