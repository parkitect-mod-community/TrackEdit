using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace RollercoasterEdit
{
    public class TrackSegmentModify : MonoBehaviour
	{
		public bool invalidate = false;

		public TrackSegment4 TrackSegment{ get; private set; }
        private List<TrackNodeCurve> nodes = new List<TrackNodeCurve> ();

        private FieldInfo biNormalField;

        private bool isSupportsInvalid = false;
        private float supportRegnerateTime = 0.0f;
        private float meshGenerationTime = 0.0f;

        void Awake()
        {
            this.TrackSegment = this.GetComponent<TrackSegment4> ();

            BindingFlags flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
            biNormalField = typeof(TrackSegment4).GetField ("startBinormal", flags);


            for (int x = 0; x < TrackSegment.curves.Count; x++) {
                TrackNodeCurve.Grouping grouping = TrackNodeCurve.Grouping.Middle;

                if (x == 0)
                    grouping = TrackNodeCurve.Grouping.Start;
                if (x == TrackSegment.curves.Count - 1)
                    grouping = TrackNodeCurve.Grouping.End;
                if (TrackSegment.curves.Count == 1)
                    grouping = TrackNodeCurve.Grouping.Both;

                nodes.Add (new TrackNodeCurve(TrackSegment.curves[x],this,grouping));

            }
            
        }

		public TrackNodeCurve getNextCurve(TrackNodeCurve current)
		{

			int index = nodes.IndexOf (current);
			if (index + 1 >= nodes.Count) {
				var nextSegment = current.SegmentModify.GetNextSegment (true);
				if (nextSegment != null)
					return nextSegment.GetFirstCurve;
				return null;
			}	
			return nodes [index + 1];
		}

		public int GetIndexOfSegment()
		{
			return TrackSegment.track.trackSegments.IndexOf (TrackSegment);
		}

		public TrackNodeCurve getPreviousCurve(TrackNodeCurve current)
		{
			int index = nodes.IndexOf (current);
			if (index - 1 < 0) {
				var previousCurve = current.SegmentModify.GetPreviousSegment (true);
				if (previousCurve != null)
					return previousCurve.GetLastCurve;
			}	
			return nodes [index - 1];
		}

		

		public List<TrackNodeCurve> GetTrackCurves{ get 
			{ 
				return nodes;
			} 
		}

		public TrackNodeCurve GetLastCurve{ get { return nodes [nodes.Count - 1];} }
		public TrackNodeCurve GetFirstCurve{ get { return nodes [0];} }

		public void CalculateStartBinormal(bool hasToBeconnected )
        {
            var previousSegment = GetPreviousSegment (true);
            if (previousSegment != null) {
                var nextSegment = GetNextSegment (true);

                biNormalField.SetValue (TrackSegment, TrackSegment.transform.InverseTransformDirection (Vector3.Cross (previousSegment.TrackSegment.getNormal (1f), previousSegment.TrackSegment.getTangentPoint (1f))));
   
                if (nextSegment != null) {

                    //try to match the curve 
                    for (int x = 0; x < 10; x++) {
                        TrackSegment.deltaRotation -= MathHelper.AngleSigned (Quaternion.AngleAxis (0, nextSegment.TrackSegment.getTangentPoint (0.0f)) * nextSegment.TrackSegment.getNormalPoint (0.0f), Quaternion.AngleAxis (TrackSegment.deltaRotation, TrackSegment.getTangentPoint (1.0f)) * TrackSegment.getNormalPoint (1.0f), TrackSegment.getTangentPoint (1.0f));
                        TrackSegment.totalRotation = previousSegment.TrackSegment.totalRotation + TrackSegment.deltaRotation;// + TrackSegment.getAdditionalRotation ();
                        TrackSegment.calculateLengthAndNormals (previousSegment.TrackSegment);
                        if (previousSegment.TrackSegment.isConnectedTo (nextSegment.TrackSegment))
                            break;
                    }
                } else {
                    TrackSegment.totalRotation = previousSegment.TrackSegment.totalRotation + TrackSegment.deltaRotation;// + TrackSegment.getAdditionalRotation ();
                    TrackSegment.calculateLengthAndNormals (previousSegment.TrackSegment);
                }
            }	

        }

       
        //calculate new rotation for segment and update the total and delta rotation
        public void CalculateWithNewTotalRotation(float newRotation)
        {
            var nextSegment = GetNextSegment (true);
            var previousSegment = GetPreviousSegment (true);

            float diff = newRotation - TrackSegment.totalRotation;
            if (previousSegment != null) {
                TrackSegment.deltaRotation += diff;
                TrackSegment.totalRotation += diff;
                TrackSegment.calculateLengthAndNormals (previousSegment.TrackSegment);

            }
            if (nextSegment != null) {
                nextSegment.CalculateStartBinormal (true);
            }
        }



		public TrackSegmentModify GetNextSegment(bool hasToBeconnected)
		{
			if (TrackSegment.isConnectedToNextSegment || !hasToBeconnected) {
                Track4 track =  TrackUIHandle.instance.trackRide.Track;
                return track.trackSegments [track.getNextSegmentIndex (track.trackSegments.IndexOf (TrackSegment))].GetComponent<TrackSegmentModify>();
			}
			return null;
		}

		public TrackSegmentModify GetPreviousSegment(bool hasToBeconnected)
		{
			if (TrackSegment.isConnectedToPreviousSegment || !hasToBeconnected) {
                Track4 track =  TrackUIHandle.instance.trackRide.Track;
                return track.trackSegments [track.getPreviousSegmentIndex (track.trackSegments.IndexOf (TrackSegment))].GetComponent<TrackSegmentModify>();
			}
			return null;

		}

        public bool ConnectWithForwardSegment(TrackSegmentModify next)
        {
            
            TrackSegment.isConnectedToNextSegment = true;
            next.TrackSegment.isConnectedToPreviousSegment = true;


            float magnitude = Mathf.Abs((next.GetFirstCurve.P0.GetGlobal () - next.GetFirstCurve.P1.GetGlobal ()).magnitude);

            GetLastCurve.P2.SetPoint(GetLastCurve.P3.GetGlobal() + (next.TrackSegment.getTangentPoint(0f) *-1f* magnitude));


            next.TrackSegment.calculateLengthAndNormals (GetLastCurve.SegmentModify.TrackSegment);
            next.CalculateStartBinormal (false);
            invalidate = true;
            return true;
        }


        void OnDestroy()
		{
			for (int x = 0; x < nodes.Count; x++) {
				nodes [x].Destroy ();
			}
		}

        private void ResetMeshForTrackSegment(MeshGenerator meshGenerator, TrackSegment4 segment)
        {
            FieldInfo generatedMesh = typeof(TrackSegment4).GetField ("generatedMeshes", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);
            List<Mesh> meshes =  (List<Mesh>)generatedMesh.GetValue (segment);
            foreach (Mesh m in meshes) {
                UnityEngine.Object.DestroyImmediate (m);
            }
            meshes.Clear ();

            foreach(Transform child in segment.gameObject.transform) {
                var mesh_filter = child.gameObject.GetComponent<MeshFilter> ();
                if (mesh_filter != null) {
                    UnityEngine.Object.DestroyImmediate (mesh_filter.mesh);
                    UnityEngine.Object.DestroyImmediate (mesh_filter.sharedMesh);
                }
                UnityEngine.Object.DestroyImmediate (child.gameObject);

            }
            UnityEngine.Object.DestroyImmediate( segment.gameObject.GetComponent<MouseCollider> ());
            UnityEngine.Object.DestroyImmediate( segment.gameObject.GetComponent<MeshCollider> ());
            UnityEngine.Object.DestroyImmediate( segment.gameObject.GetComponent<BoundingMesh> ());

        }

		void Update()
		{
            if(invalidate)
                supportRegnerateTime = Time.time;

            if (invalidate && (Time.time - meshGenerationTime) > .05f) {
                isSupportsInvalid = true;

                ResetMeshForTrackSegment (TrackUIHandle.instance.trackRide.meshGenerator, TrackSegment);
                TrackSegment.generateMesh (TrackUIHandle.instance.trackRide.meshGenerator);

                meshGenerationTime = Time.time;
				invalidate = false;
			}
            if (isSupportsInvalid && (Time.time - supportRegnerateTime) > .1f) {
               
                for (int x = 0; x < 5; x++) {
                    ResetMeshForTrackSegment (TrackUIHandle.instance.trackRide.meshGenerator, TrackSegment);
                    FieldInfo localCrossTile = typeof(TrackSegment4).GetField ("localCrossedTiles", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);
                    localCrossTile.SetValue (TrackSegment, null);

                    TrackSegment.generateMesh (TrackUIHandle.instance.trackRide.meshGenerator);
                }
                isSupportsInvalid = false;
            }
            foreach (TrackNodeCurve curves in nodes) {
                curves.Update ();
            }

		}




	}
}

