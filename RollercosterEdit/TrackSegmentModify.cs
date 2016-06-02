using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace RollercoasterEdit
{
    public class TrackSegmentModify : MonoBehaviour
	{
		public bool Invalidate = false;

		public TrackSegment4 TrackSegment{ get; private set; }
		private List<TrackNodeCurve> _nodes = new List<TrackNodeCurve> ();

		private FieldInfo _biNormalField;

        void Awake()
        {
            this.TrackSegment = this.GetComponent<TrackSegment4> ();

            BindingFlags flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
            _biNormalField = typeof(TrackSegment4).GetField ("startBinormal", flags);


            for (int x = 0; x < TrackSegment.curves.Count; x++) {
                TrackNodeCurve.Grouping grouping = TrackNodeCurve.Grouping.Middle;

                if (x == 0)
                    grouping = TrackNodeCurve.Grouping.Start;
                if (x == TrackSegment.curves.Count - 1)
                    grouping = TrackNodeCurve.Grouping.End;
                if (TrackSegment.curves.Count == 1)
                    grouping = TrackNodeCurve.Grouping.Both;

                _nodes.Add (new TrackNodeCurve(TrackSegment.curves[x],this,grouping));

            }
            
        }

		public TrackNodeCurve getNextCurve(TrackNodeCurve current)
		{

			int index = _nodes.IndexOf (current);
			if (index + 1 >= _nodes.Count) {
				var nextSegment = current.SegmentModify.GetNextSegment (true);
				if (nextSegment != null)
					return nextSegment.GetFirstCurve;
				return null;
			}	
			return _nodes [index + 1];
		}

		public int GetIndexOfSegment()
		{
			return TrackSegment.track.trackSegments.IndexOf (TrackSegment);
		}

		public TrackNodeCurve getPreviousCurve(TrackNodeCurve current)
		{
			int index = _nodes.IndexOf (current);
			if (index - 1 < 0) {
				var previousCurve = current.SegmentModify.GetPreviousSegment (true);
				if (previousCurve != null)
					return previousCurve.GetLastCurve;
			}	
			return _nodes [index - 1];
		}

		

		public List<TrackNodeCurve> GetTrackCurves{ get 
			{ 
				return _nodes;
			} 
		}

		public TrackNodeCurve GetLastCurve{ get { return _nodes [_nodes.Count - 1];} }
		public TrackNodeCurve GetFirstCurve{ get { return _nodes [0];} }

		public void CalculateStartBinormal(bool hasToBeconnected )
        {
            var previousSegment = GetPreviousSegment (true);
            if (previousSegment != null) {
                var nextSegment = GetNextSegment (true);

                _biNormalField.SetValue (TrackSegment, TrackSegment.transform.InverseTransformDirection (Vector3.Cross (previousSegment.TrackSegment.getNormal (1f), previousSegment.TrackSegment.getTangentPoint (1f))));
                //TrackSegment.calculateLengthAndNormals (previousSegment.TrackSegment);
                //GetLastCurve.P0.CalculateLenghtAndNormals ();

                if (nextSegment != null) {

                    TrackSegment.deltaRotation -= AngleSigned(Quaternion.AngleAxis (0, nextSegment.TrackSegment.getTangentPoint (0.0f)) * nextSegment.TrackSegment.getNormalPoint (0.0f), Quaternion.AngleAxis ( TrackSegment.deltaRotation ,TrackSegment.getTangentPoint (1.0f)) * TrackSegment.getNormalPoint (1.0f),TrackSegment.getTangentPoint(1.0f)) ;

                    //Vector3 normalPoint = TrackSegment.getNormalPoint (1.0f);


                    //TrackSegment.deltaRotation = Mathf.DeltaAngle( previousSegment.TrackSegment.totalRotation,nextSegment.TrackSegment.totalRotation - nextSegment.TrackSegment.deltaRotation -nextSegment.TrackSegment.getAdditionalRotation()) - TrackSegment.getAdditionalRotation();

                }
                TrackSegment.totalRotation = previousSegment.TrackSegment.totalRotation + TrackSegment.deltaRotation + TrackSegment.getAdditionalRotation ();
                TrackSegment.calculateLengthAndNormals (previousSegment.TrackSegment);
            }	

        }

        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                Vector3.Dot(n, Vector3.Cross(v1, v2)),
                Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }


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
                //TrackSegment.calculateLengthAndNormals (previousSegment.TrackSegment);
            }
        }



		public TrackSegmentModify GetNextSegment(bool hasToBeconnected)
		{
			if (TrackSegment.isConnectedToNextSegment || !hasToBeconnected) {
                Track4 track =  TrackUIHandle.instance.TrackRide.Track;
                return track.trackSegments [track.getNextSegmentIndex (track.trackSegments.IndexOf (TrackSegment))].GetComponent<TrackSegmentModify>();
			}
			return null;
		}

		public TrackSegmentModify GetPreviousSegment(bool hasToBeconnected)
		{
			if (TrackSegment.isConnectedToPreviousSegment || !hasToBeconnected) {
                Track4 track =  TrackUIHandle.instance.TrackRide.Track;
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
            GetLastCurve.P2.CalculateLenghtAndNormals ();

            Invalidate = true;
            return true;
        }


        void OnDestroy()
		{
			for (int x = 0; x < _nodes.Count; x++) {
				_nodes [x].Destroy ();
			}
		}

		void Update()
		{
			if (Invalidate) {
                
                recalculate(TrackUIHandle.instance.TrackRide.meshGenerator,TrackSegment);
				Invalidate = false;
			}
            foreach (TrackNodeCurve curves in _nodes) {
                curves.Update ();
            }

		}


		private void recalculate(MeshGenerator meshGenerator, TrackSegment4 segment)
		{

            foreach(Transform child in segment.gameObject.transform) {
                if (child.name != "BetweenTracksMouseCollider" && !child.name.Contains("StationPlatformTrackTile") && child.name != "MouseSelectionCollider") {
                    var mesh_filter = child.gameObject.GetComponent<MeshFilter> ();
                    if (mesh_filter != null) {
                        UnityEngine.Object.Destroy (mesh_filter.mesh);
                        UnityEngine.Object.Destroy (mesh_filter.sharedMesh);
                    }
                    UnityEngine.Object.Destroy (child.gameObject);

                }
            }


            if (segment.getLength() <= 0f)
            {
                Debug.LogWarning("Can't extrude this segment! Has a length of 0.");
            }
            meshGenerator.prepare(segment, segment.gameObject);
            float num = 0f;
            float num2 = 0f;
            meshGenerator.sampleAt(segment, 0f);
            int num3 = 0;
            int num4 = 0;
            Vector3 b = segment.getStartpoint();
            do
            {
                float num5 = 1f - num2;
                if (Vector3.Angle(segment.getDirection(), segment.getPoint(num2 + num5) - segment.getPoint(num2)) > 5f)
                {
                    num5 /= 2f;
                }
                int num6 = 0;
                Vector3 point = segment.getPoint(num2 + num5);
                float num7 = Vector3.Angle(segment.getTangentPoint(num2), segment.getTangentPoint(num2 + num5));
                num7 = Mathf.Max(num7, Vector3.Angle(segment.getNormal(num2), segment.getNormal(num2 + num5)));
                while (num5 > 0.01f && (num7 > 10f || (num7 > 2f && (point - b).magnitude > 0.225f)))
                {
                    num4++;
                    num5 /= 2f;
                    point = segment.getPoint(num2 + num5);
                    num7 = Vector3.Angle(segment.getTangentPoint(num2), segment.getTangentPoint(num2 + num5));
                    num7 = Mathf.Max(num7, Vector3.Angle(segment.getNormal(num2), segment.getNormal(num2 + num5)));
                    num6++;
                    if (num6 > 50)
                    {
                        break;
                    }
                }
                num += (point - b).magnitude;
                num2 += num5;
                b = point;
                if (num2 > 1f)
                {
                    break;
                }
                meshGenerator.sampleAt(segment, num2);
                num3++;
            }
            while (num2 < 1f && num3 < 300);
            if (!Mathf.Approximately(num2, 1f))
            {
                meshGenerator.sampleAt(segment, 1f);
            }

            meshGenerator.afterExtrusion(segment, segment.gameObject);
            MeshFilter component = segment.gameObject.GetComponent<MeshFilter>();
            Mesh mesh = meshGenerator.getMesh(segment.gameObject);
            UnityEngine.Object.Destroy (component.sharedMesh);
            UnityEngine.Object.Destroy (component.mesh);

            component.sharedMesh = mesh;
            meshGenerator.afterMeshGeneration(segment, segment.gameObject);

            Extruder buildVolumeMeshExtruder = meshGenerator.getBuildVolumeMeshExtruder();
            buildVolumeMeshExtruder.transform(segment.gameObject.transform.worldToLocalMatrix);
            BoundingMesh boundingMesh = segment.gameObject.GetComponent<BoundingMesh>();
            boundingMesh.layers = BoundingVolume.Layers.Buildvolume;
            boundingMesh.setMesh(buildVolumeMeshExtruder.vertices.ToArray(), buildVolumeMeshExtruder.indizes.ToArray());

            GameObject track_mouse_collider = segment.transform.Find ("BetweenTracksMouseCollider").gameObject;// new GameObject("BetweenTracksMouseCollider");
            track_mouse_collider.transform.parent = segment.gameObject.transform;
            track_mouse_collider.transform.localPosition = Vector3.zero;
            track_mouse_collider.transform.localRotation = Quaternion.identity;
            track_mouse_collider.layer = LayerMasks.ID_MOUSECOLLIDERS;
            MeshCollider meshCollider = track_mouse_collider.GetComponent<MeshCollider>();
            Mesh collisionMesh = meshGenerator.getCollisionMesh(segment.gameObject);

            UnityEngine.Object.Destroy (meshCollider.sharedMesh);
            meshCollider.sharedMesh = collisionMesh;

            MouseCollider mouseCollider = segment.gameObject.GetComponent<MouseCollider>();
            mouseCollider.colliderObject = track_mouse_collider;


            segment.applyCustomColors ((Color[])segment.track.TrackedRide.trackColors.Clone ());

		}



	}
}

