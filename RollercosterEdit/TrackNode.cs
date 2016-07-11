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

		public Activestate activeState = Activestate.Default;
		public NodeType nodePoint;
		public CubicBezier curve;
		public TrackSegmentModify trackSegmentModify ;
		public TrackNodeCurve trackCurve;
        private LineRenderer lineSegment;
        public RotationNode rotate;

		public void ActivateNeighbors(bool active)
		{
			TrackNodeCurve nextCurve = trackSegmentModify.getNextCurve (trackCurve);
            TrackNodeCurve previousCurve = trackSegmentModify.getPreviousCurve (trackCurve);
            if (nodePoint != NodeType.P3) {
                this.SetActiveState (active);

            }
			switch (nodePoint) {
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
				trackCurve.P3.SetActiveState (active);
				if (nextCurve != null) {
					nextCurve.P1.SetActiveState (active);
				}
				break;
            case NodeType.P3:
                trackCurve.P2.SetActiveState (active);
                if (nextCurve != null) {
                    nextCurve.P1.SetActiveState (active);
                }
				break;

			}
            if (rotate != null)
                rotate.transform.parent.gameObject.SetActive (active);

            
			
		}

		void Start()
		{
			
			if (nodePoint == NodeType.P3 ) {
                lineSegment = this.gameObject.transform.FindChild("item").gameObject.GetComponent<LineRenderer> ();

			}
		}

		public void Initialize()
		{
		}

		void Update()
		{
			if (nodePoint == NodeType.P3) 
			{
				var nextCurve = trackSegmentModify.getNextCurve (trackCurve);
				if (nextCurve != null) {
                    Vector3 v1 = this.transform.FindChild ("item").position;
                    Vector3 v2 = this.transform.FindChild ("item").position;
                    Vector3 v3 = this.transform.FindChild ("item").position;

					if (nextCurve != null && nextCurve.P1.isActiveAndEnabled) {
                        v1 = nextCurve.P1.transform.FindChild ("item").position;

					}
					if (trackCurve.P2.isActiveAndEnabled) {
                        v3 = trackCurve.P2.transform.FindChild ("item").position;
					}


					lineSegment.SetPositions (new Vector3[] {
						v1,
						v2,
						v3
					});
				}
					
			}

            //error checking to mark bad nodes
            TrackSegmentModify next = this.trackSegmentModify.GetNextSegment (true);
            if (next != null && !this.trackSegmentModify.TrackSegment.isConnectedTo (next.TrackSegment)) 
                this.transform.FindChild("item").GetComponent<Renderer> ().material.color = new Color (1,0, 0, .5f);
            else
                this.transform.FindChild("item").GetComponent<Renderer> ().material.color = new Color (1,1, 1, .5f);

			this.transform.FindChild("item").LookAt(Camera.main.transform,Vector3.down) ;
		}

		public void SetActiveState(bool active)
		{
           
            
			if (this.activeState == Activestate.AlwaysActive) {
				this.gameObject.SetActive (true);
			} else if (this.activeState == Activestate.NeverActive) {
                this.gameObject.SetActive (false);
			} else if (this.activeState == Activestate.Default) {
				this.gameObject.SetActive (active);
			}
            if (rotate != null)
                rotate.transform.parent.gameObject.SetActive (active);
		}

		public void SetPoint(Vector3 point)
		{
			Vector3 p = trackSegmentModify.TrackSegment.transform.InverseTransformPoint (point) ;

			switch (nodePoint) {
			case NodeType.PO:
				curve.p0 = p;
				break;
			case NodeType.P1:
				curve.p1 =p;
				break;
			case NodeType.P2:
				curve.p2 =p;
				break;
			case NodeType.P3:
				curve.p3 =p;
				break;
			}
			this.transform.position = point;
		}

		public Vector3 GetLocal()
		{
			return trackSegmentModify.TrackSegment.transform.InverseTransformPoint (this.transform.position);
		}

		public Vector3 GetGlobal()
		{

			switch (nodePoint) {
			case NodeType.PO:
				
				return trackSegmentModify.TrackSegment.transform.TransformPoint(curve.p0);
		
			case NodeType.P1:
				return trackSegmentModify.TrackSegment.transform.TransformPoint(curve.p1);

			case NodeType.P2:
				return trackSegmentModify.TrackSegment.transform.TransformPoint(curve.p2);

			case NodeType.P3:
				return trackSegmentModify.TrackSegment.transform.TransformPoint(curve.p3);

			}
			return Vector3.zero;
		}

		public void UpdatePosition()
		{
				switch (nodePoint) {
				case NodeType.PO:
					this.transform.position = trackSegmentModify.TrackSegment.transform.TransformPoint (curve.p0) ;
					break;
				case NodeType.P1:
					this.transform.position = trackSegmentModify.TrackSegment.transform.TransformPoint (curve.p1) ;
					break;
				case NodeType.P2:
					this.transform.position = trackSegmentModify.TrackSegment.transform.TransformPoint (curve.p2) ;
					break;
				case NodeType.P3:
					this.transform.position = trackSegmentModify.TrackSegment.transform.TransformPoint (curve.p3) ;
					break;
				}

	
		}


        public void CalculateLenghtAndNormals()
		{
			var nextSegment = trackSegmentModify.GetNextSegment (true);
			var previousSegment = trackSegmentModify.GetPreviousSegment (true);

			if(previousSegment != null)
				previousSegment.TrackSegment.calculateLengthAndNormals (trackSegmentModify.TrackSegment);

			if(nextSegment != null)
				trackSegmentModify.TrackSegment.calculateLengthAndNormals (nextSegment.TrackSegment);

			if(nextSegment != null)
				nextSegment.TrackSegment.calculateLengthAndNormals (trackSegmentModify.TrackSegment);
		}


	}
}

