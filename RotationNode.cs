using UnityEngine;

namespace TrackEdit
{
    public class RotationNode : MonoBehaviour, INode
    {
        public TrackNode AttachedNode { get; set; }

        private void Start()
        {
            AttachedNode = transform.parent.parent.GetComponent<TrackNode>();
            transform.parent.gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.parent.position = AttachedNode.transform.position;
            transform.parent.rotation =
                Quaternion.LookRotation(AttachedNode.TrackSegmentModify.TrackSegment.getTangentPoint(1f));

            transform.localEulerAngles = new Vector3(0, 0, AttachedNode.TrackSegmentModify.TrackSegment.totalRotation);
            transform.parent.Find("Angle").GetComponent<TextMesh>().text =
                AttachedNode.TrackSegmentModify.TrackSegment.totalRotation % 360 + "\u00B0";
            transform.parent.Find("Angle").LookAt(Camera.main.transform, Vector3.up);
        }
    }
}