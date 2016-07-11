using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class RotationState : IState
    {
        private SharedStateData stateData;
        public RotationState (SharedStateData stateData)
        {
            this.stateData = stateData;
        }

        public void Update (FiniteStateMachine stateMachine)
        {
            var ray = Camera.main.ScreenPointToRay (Input.mousePosition);

            RotationNode rotationNode = stateData.Selected.GetComponent<RotationNode>();

            Vector3 planeNormal = rotationNode.AttachedNode.trackSegmentModify.TrackSegment.getTangentPoint (1.0f);
            Vector3 planeCenter = rotationNode.transform.position;

            float l = planeNormal.x * ray.origin.x + planeNormal.y * ray.origin.y + planeNormal.z * ray.origin.z;
            float r = planeNormal.x * planeCenter.x + planeNormal.y * planeCenter.y + planeCenter.z * planeNormal.z;

            float t = (-l + r) / (planeNormal.x * ray.direction.x + planeNormal.y * ray.direction.y + planeNormal.z * ray.direction.z);

            Vector3 loc = ray.origin + ray.direction * t;
            float diff = MathHelper.AngleSigned(rotationNode.AttachedNode.trackSegmentModify.TrackSegment.getNormal (1.0f),Vector3.Normalize (planeCenter- loc),planeNormal);

            rotationNode.AttachedNode.trackSegmentModify.CalculateWithNewTotalRotation ( Mathf.Round(diff + rotationNode.AttachedNode.trackSegmentModify.TrackSegment.totalRotation));
            rotationNode.AttachedNode.trackSegmentModify.invalidate = true;

            TrackSegmentModify nextSegment = rotationNode.AttachedNode.trackSegmentModify.GetNextSegment (true);

            if(nextSegment != null)
                nextSegment.invalidate = true;
       
            if (Input.GetMouseButtonUp (0)) {
                stateMachine.ChangeState(new IdleState (stateData));
            }
        }


        public void Unload ()
        {
        }
    }
}

