using System;
using UnityEngine;
using System.Collections.Generic;

namespace HelloMod
{
	public class BezierNodes
	{
		private CubicBezier _cubicBezier;
		private PreciseModify _modify;
		private TrackSegment4 _segment;

		public Node P0;
		public Node P1;
		public Node P2;
		public Node P3;

		public BezierNodes (TrackSegment4 segment,CubicBezier cubicBezier, PreciseModify preciseModify,bool beginning)
		{
			_cubicBezier = cubicBezier;
			_modify = preciseModify;
			_segment = segment;



			if(!beginning)
				P0 = AddNode (_modify.transform, segment.transform.TransformPoint (_cubicBezier.p0),Node.NodeType.PO);
			
			P1 = AddNode (_modify.transform,  segment.transform.TransformPoint (_cubicBezier.p1),Node.NodeType.P1);
			P2 = AddNode (_modify.transform,  segment.transform.TransformPoint (_cubicBezier.p2),Node.NodeType.P2);
			P3 = AddNode (_modify.transform, segment.transform.TransformPoint (_cubicBezier.p3),Node.NodeType.P3);

		}

		public void Destroy()
		{
			if(P0 != null)
			UnityEngine.Object.Destroy (P0.gameObject);
			if(P1 != null)
			UnityEngine.Object.Destroy (P1.gameObject);
			if(P2 != null)
			UnityEngine.Object.Destroy (P2.gameObject);
			if(P3 != null)
			UnityEngine.Object.Destroy (P3.gameObject);

		}

		private Node AddNode(Transform parent, Vector3 position, Node.NodeType type)
		{
			GameObject node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			node.transform.name = "BezierNode";
			node.layer = 900;
			node.transform.parent = parent;
			node.transform.transform.position = position;
			var n = node.AddComponent< Node>();
			n.Curve = _cubicBezier;
			n.Segment = _segment;
			n.NodePoint = type;
			n.PreciseModify = _modify;
		
			return n;
		}

	
	}
}

