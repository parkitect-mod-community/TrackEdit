using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class RotationNode: MonoBehaviour, INode
    {
        public TrackNode AttachedNode{ get; set; }

        void Start()
        {
            AttachedNode = this.transform.parent.parent.GetComponent<TrackNode> ();
            this.transform.parent.gameObject.SetActive (false);
        }

        void Update()
        {
            this.transform.parent.position =  AttachedNode.transform.position;
            this.transform.parent.rotation =  Quaternion.LookRotation( AttachedNode.trackSegmentModify.TrackSegment.getTangentPoint (1f));

            this.transform.localEulerAngles = new Vector3( 0,0,AttachedNode.trackSegmentModify.TrackSegment.totalRotation);
            this.transform.parent.Find ("Angle").GetComponent<TextMesh> ().text = (AttachedNode.trackSegmentModify.TrackSegment.totalRotation % 360) + "\u00B0"; 
            this.transform.parent.Find("Angle").LookAt(Camera.main.transform,Vector3.up) ;
        }
    }
}
