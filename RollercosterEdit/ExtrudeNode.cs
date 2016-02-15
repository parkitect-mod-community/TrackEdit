using System;
using UnityEngine;

namespace RollercoasterEdit
{
	public class ExtrudeNode : MonoBehaviour, INode 
	{
		public CubicBezier Curve;
		public TrackSegmentModify TrackSegmentModify ;
		public TrackNodeCurve TrackCurve;

		public ExtrudeNode ()
		{
		}

		void Update()
		{
			this.transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p3) + TrackSegmentModify.TrackSegment.getTangentPoint (1f) * .3f;

			this.transform.FindChild("item").GetComponent<Renderer> ().material.color = new Color (0,1, 0, .5f);
			this.transform.FindChild("item").LookAt(Camera.main.transform,Vector3.down) ;
		}
	}
}

