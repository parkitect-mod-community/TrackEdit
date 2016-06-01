using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class RotationNode: MonoBehaviour, INode
    {
        public TrackNode AttachedNode{ get;set;}

        void Start()
        {
            AttachedNode = this.transform.parent.parent.GetComponent<TrackNode> ();
            this.gameObject.SetActive (false);
        }

        void Update()
        {
            this.transform.parent.position =  AttachedNode.transform.position;
            this.transform.parent.rotation =  Quaternion.LookRotation( AttachedNode.TrackSegmentModify.TrackSegment.getTangentPoint (1f) * -1.0f);

            this.transform.localEulerAngles = new Vector3( 0,0,AttachedNode.TrackSegmentModify.TrackSegment.totalRotation);
        }
    }
}
