using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class SharedStateData
    {
		public TrackSegmentManager SegmentManager;

        public Transform Selected;
        public Vector3 Offset;
        public float OffsetFixedY;

        public float Distance;

        public float FixedY;
    }
}

