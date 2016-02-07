using System;
using System.Collections.Generic;
using UnityEngine;

namespace HelloMod
{
	public class TrackSegmentModify
	{
		public bool Invalidate = false;

		public TrackSegment4 TrackSegment{ get; private set; }
		public List<TrackNodeCurve> _nodes = new List<TrackNodeCurve> ();
		public TrackUIHandle _trackUIHandler;
		public TrackSegmentModify (TrackSegment4 segment,TrackUIHandle trackUIHandler)
		{
			this._trackUIHandler = trackUIHandler;
			this.TrackSegment = segment;

			for (int x = 0; x < TrackSegment.curves.Count; x++) {
				if(0 == x)
					_nodes.Add (new TrackNodeCurve(TrackSegment.curves[x],this,true));
				else
					_nodes.Add (new TrackNodeCurve(TrackSegment.curves[x],this,false));


			}

		}

		public TrackSegmentModify GetNextSegment()
		{
			if (TrackSegment.isConnectedToNextSegment) {
				var trackSegment =	_trackUIHandler.TrackRide.Track.trackSegments[_trackUIHandler.TrackRide.Track.getNextSegmentIndex (_trackUIHandler.TrackRide.Track.trackSegments.IndexOf (TrackSegment))];
				return _trackUIHandler.GetSegment (trackSegment);
			}
			return null;
		}

		public TrackSegmentModify GetPreviousSegment()
		{
			if (TrackSegment.isConnectedToPreviousSegment) {
				var trackSegment =	_trackUIHandler.TrackRide.Track.trackSegments[_trackUIHandler.TrackRide.Track.getPreviousSegmentIndex (_trackUIHandler.TrackRide.Track.trackSegments.IndexOf (TrackSegment))];
				return _trackUIHandler.GetSegment (trackSegment);
			}
			return null;

		}


		public void Destroy()
		{
			for (int x = 0; x < _nodes.Count; x++) {
				_nodes [x].Destroy ();
			}
		}

		public void Update()
		{
			if (Invalidate) {
				recalculate(_trackUIHandler.TrackRide.meshGenerator,TrackSegment);

				Invalidate = false;
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

		}



	}
}

