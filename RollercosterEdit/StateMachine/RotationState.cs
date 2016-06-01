using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class RotationState : IState
    {
        private SharedStateData _stateData;
        public RotationState (SharedStateData stateData)
        {
            this._stateData = stateData;
        }

        public void Update (FiniteStateMachine stateMachine)
        {
            var ray = Camera.main.ScreenPointToRay (Input.mousePosition);

            RotationNode rotationNode = _stateData.Selected.GetComponent<RotationNode>();

            Vector3 planeNormal = rotationNode.AttachedNode.TrackSegmentModify.TrackSegment.getTangentPoint (1.0f);
            Vector3 planeCenter = rotationNode.transform.position;

            float l = planeNormal.x * ray.origin.x + planeNormal.y * ray.origin.y + planeNormal.z * ray.origin.z;
            float r = planeNormal.x * planeCenter.x + planeNormal.y * planeCenter.y + planeCenter.z * planeNormal.z;

            float t = (-l + r) / (planeNormal.x * ray.direction.x + planeNormal.y * ray.direction.y + planeNormal.z * ray.direction.z);

            Vector3 loc = ray.origin + ray.direction * t;
            float diff = -AngleSigned(rotationNode.AttachedNode.TrackSegmentModify.TrackSegment.getNormal (1.0f),Vector3.Normalize (planeCenter- loc),planeNormal);

            rotationNode.AttachedNode.TrackSegmentModify.CalculateWithNewTotalRotation (diff + rotationNode.AttachedNode.TrackSegmentModify.TrackSegment.totalRotation);
            rotationNode.AttachedNode.TrackSegmentModify.Invalidate = true;
            rotationNode.AttachedNode.TrackSegmentModify.GetNextSegment (true).Invalidate = true;
       
            if (Input.GetMouseButtonUp (0)) {
                stateMachine.ChangeState(new IdleState (_stateData));
            }
        }

        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        public void Unload ()
        {
        }
    }
}

