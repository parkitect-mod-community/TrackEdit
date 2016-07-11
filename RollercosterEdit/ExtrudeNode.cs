using System;
using UnityEngine;

namespace RollercoasterEdit
{
	public class ExtrudeNode : MonoBehaviour, INode 
	{
		public CubicBezier curve;
		public TrackSegmentModify trackSegmentModify ;
		public TrackNodeCurve trackCurve;

		public ExtrudeNode ()
		{
		}

		void Update()
		{
			this.transform.position = trackSegmentModify.TrackSegment.transform.TransformPoint (curve.p3) + trackSegmentModify.TrackSegment.getTangentPoint (1f) * .3f;

			this.transform.FindChild("item").GetComponent<Renderer> ().material.color = new Color (0,1, 0, .5f);
			this.transform.FindChild("item").LookAt(Camera.main.transform,Vector3.down) ;
		}
	}
}

