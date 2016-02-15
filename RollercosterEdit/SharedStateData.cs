using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class SharedStateData
    {
		public TrackSegmentManager SegmentManager;

		public Transform ActiveNode{ get;private set;}
		public void SetActiveNode(Transform active)
		{
			if (active != ActiveNode) {
				if (ActiveNode != null)
					ActiveNode.GetComponent<TrackNode> ().ActivateNeighbors (false);
				if (active != null)
					active.GetComponent<TrackNode> ().ActivateNeighbors (true);
			}
			this.ActiveNode = active;
		}

        public Transform Selected;
        public Vector3 Offset;

        public float Distance;

        public float FixedY;
    }
}

