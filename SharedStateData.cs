using UnityEngine;

namespace TrackEdit
{
    public class SharedStateData
    {
        public float Distance;

        public float FixedY;
        public Vector3 Offset;

        public Transform Selected;
        public Transform ActiveNode { get; private set; }

        public void SetActiveNode(Transform active)
        {
            if (active != ActiveNode)
            {
                if (ActiveNode != null)
                    ActiveNode.GetComponent<TrackNode>().ActivateNeighbors(false);
                if (active != null)
                    active.GetComponent<TrackNode>().ActivateNeighbors(true);
            }

            ActiveNode = active;
        }
    }
}