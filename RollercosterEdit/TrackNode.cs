using System;
using UnityEngine;

namespace RollercoasterEdit
{
	public class TrackNode : MonoBehaviour, INode
	{
		public enum NodeType  {
			PO,
			P1,
			P2,
			P3
		};

		public enum Activestate{
			AlwaysActive,
			Default,
			NeverActive
		}

		public Activestate ActiveState = Activestate.Default;
		public NodeType NodePoint;
		public CubicBezier Curve;
		public TrackSegmentModify TrackSegmentModify ;
		public TrackNodeCurve TrackCurve;

		private LineRenderer _lineSegment;

		private Vector3 _previousPos = new Vector3(); 

		public void ActivateNeighbors(bool active)
		{
			TrackNodeCurve nextCurve = TrackSegmentModify.getNextCurve (TrackCurve);
			TrackNodeCurve previousCurve = TrackSegmentModify.getPreviousCurve (TrackCurve);
			if(NodePoint != NodeType.P3)
				this.SetActiveState (active);

			switch (NodePoint) {
			case NodeType.PO:
				//P0 Node is never active
				break;
			case NodeType.P1:
					//TrackCurve.P0.gameObject.SetActive (active);
				if (previousCurve != null) {
					previousCurve.P2.SetActiveState (active);
					previousCurve.P3.SetActiveState (active);
				}

				break;
			case NodeType.P2:
				TrackCurve.P3.SetActiveState (active);
				if (nextCurve != null) {
					nextCurve.P1.SetActiveState (active);
				}
				break;
			case NodeType.P3:
				TrackCurve.P2.SetActiveState (active);
				if (nextCurve != null) {
					nextCurve.P1.SetActiveState (active);
				}
				break;

			}
			
		}

		public TrackNode ()
		{
			
		}

		void Start()
		{
			
			if (NodePoint == NodeType.P3 ) {
				_lineSegment = this.gameObject.transform.Find("item").gameObject.GetComponent<LineRenderer> ();

			}
		}

		public void Initialize()
		{
		}

		void Update()
		{
			if (NodePoint == NodeType.P3) 
			{
				var nextCurve = TrackSegmentModify.getNextCurve (TrackCurve);
				if (nextCurve != null) {
					Vector3 v1 = this.transform.Find ("item").position;
					Vector3 v2 = this.transform.Find ("item").position;
					Vector3 v3 = this.transform.Find ("item").position;

					if (nextCurve != null && nextCurve.P1.isActiveAndEnabled) {
						v1 = nextCurve.P1.transform.Find ("item").position;

					}
					if (TrackCurve.P2.isActiveAndEnabled) {
						v3 = TrackCurve.P2.transform.Find ("item").position;
					}

					_lineSegment.SetPositions (new Vector3[] {
						v1,
						v2,
						v3
					});
				}
					
			}
			this.transform.FindChild("item").GetComponent<Renderer> ().material.color = new Color (1,1, 1, .5f);
			this.transform.FindChild("item").LookAt(Camera.main.transform,Vector3.down) ;
		}

		public void SetActiveState(bool active)
		{
			if (this.ActiveState == Activestate.AlwaysActive) {
				this.gameObject.SetActive (true);
			} else if (this.ActiveState == Activestate.NeverActive) {
				this.gameObject.SetActive (false);
			} else if (this.ActiveState == Activestate.Default) {
				this.gameObject.SetActive (active);
			}
		}

		public void SetPoint(Vector3 point)
		{
			Vector3 p = TrackSegmentModify.TrackSegment.transform.InverseTransformPoint (point) ;

			switch (NodePoint) {
			case NodeType.PO:
				_previousPos = Curve.p0;
				Curve.p0 = p;
				break;
			case NodeType.P1:
				_previousPos = Curve.p1;
				Curve.p1 =p;
				break;
			case NodeType.P2:
				_previousPos = Curve.p2;
				Curve.p2 =p;
				break;
			case NodeType.P3:
				_previousPos = Curve.p3;
				Curve.p3 =p;
				break;
			}
			this.transform.position = point;
		}

		public Vector3 GetLocal()
		{
			return TrackSegmentModify.TrackSegment.transform.InverseTransformPoint (this.transform.position);
		}

		public Vector3 GetGlobal()
		{

			switch (NodePoint) {
			case NodeType.PO:
				
				return TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p0);
		
			case NodeType.P1:
				return TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p1);

			case NodeType.P2:
				return TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p2);

			case NodeType.P3:
				return TrackSegmentModify.TrackSegment.transform.TransformPoint(Curve.p3);

			}
			return Vector3.zero;
		}

		public void RollBack()
		{
			if (_previousPos != Vector3.zero) {
				switch (NodePoint) {
				case NodeType.PO:
					Curve.p0 = _previousPos ;

					break;
				case NodeType.P1:
					Curve.p1 = _previousPos;
					break;
				case NodeType.P2:
					Curve.p2 = _previousPos ;
					break;
				case NodeType.P3:
					Curve.p3 = _previousPos ;
					break;
				}
			}
		}

		public void UpdatePosition()
		{
				switch (NodePoint) {
				case NodeType.PO:
					this.transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p0) ;
					break;
				case NodeType.P1:
					this.transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p1) ;
					break;
				case NodeType.P2:
					this.transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p2) ;
					break;
				case NodeType.P3:
					this.transform.position = TrackSegmentModify.TrackSegment.transform.TransformPoint (Curve.p3) ;
					break;
				}

	
		}


        public void CalculateLenghtAndNormals()
		{
			var nextSegment = TrackSegmentModify.GetNextSegment (true);
			var previousSegment = TrackSegmentModify.GetPreviousSegment (true);


			if(previousSegment != null)
				previousSegment.TrackSegment.calculateLengthAndNormals (TrackSegmentModify.TrackSegment);

			if(nextSegment != null)
				TrackSegmentModify.TrackSegment.calculateLengthAndNormals (nextSegment.TrackSegment);

			if(nextSegment != null)
				nextSegment.TrackSegment.calculateLengthAndNormals (TrackSegmentModify.TrackSegment);
		}

		public bool Validate()
		{
			bool isValid = true;
			var nextSegment = TrackSegmentModify.GetNextSegment (true);
			var previousSegment = TrackSegmentModify.GetPreviousSegment (true);

			CalculateLenghtAndNormals ();

			if (previousSegment != null) {
				if (!previousSegment.TrackSegment.isConnectedTo (TrackSegmentModify.TrackSegment)) {
					previousSegment.RollBackSegment ();
					TrackSegmentModify.RollBackSegment ();
					isValid = false;
				}
			}
			if (nextSegment != null) {
				if (!TrackSegmentModify.TrackSegment.isConnectedTo (nextSegment.TrackSegment)) {
					TrackSegmentModify.RollBackSegment ();
					nextSegment.RollBackSegment ();
					isValid = false;

				}
			}

			CalculateLenghtAndNormals ();
			return isValid;

		}


	}
}

