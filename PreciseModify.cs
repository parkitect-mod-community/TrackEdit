using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace HelloMod
{
	public class PreciseModify : MonoBehaviour
	{
		private Rect _windowRect = new Rect(20, 20, 1000, 500);
		private Rect _titleBarRect = new Rect(0, 0, 200000000, 20);

		private int _trackPosition;
		private FieldInfo _trackCurrentPositionField;
		private FieldInfo _trackerRiderField;
		private TrackBuilder _trackBuilder;

		private TrackedRide _trackRide;

		private List<VectorUI> _vectorUI = new List<HelloMod.VectorUI>();

		public PreciseModify ()
		{
			
		}

		void Awake()
		{
			_trackBuilder = this.gameObject.GetComponent<TrackBuilder>();
			BindingFlags flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
			_trackCurrentPositionField = _trackBuilder.GetType ().GetField ("trackCursorPosition", flags);
			_trackerRiderField = _trackBuilder.GetType ().GetField ("trackedRide", flags);
		
		}

		void Update()
		{
			if ((int)_trackCurrentPositionField.GetValue(_trackBuilder)  != _trackPosition) {
				_vectorUI.Clear ();
				_trackPosition = (int)_trackCurrentPositionField.GetValue (_trackBuilder);
				_trackRide = ((TrackedRide)_trackerRiderField.GetValue (_trackBuilder));
				for (int x = 0; x < _trackRide.Track.trackSegments[_trackPosition].curves.Count; x++) {
					_vectorUI.Add (new HelloMod.VectorUI(_trackRide.Track.trackSegments[_trackPosition].curves [x]));
				}
			}




		}
			
		void OnGUI()
		{
			if (_trackRide != null )
                _windowRect = GUILayout.Window(45051 , _windowRect, DrawMain, "Bezier Modifer");

		}
		public void DrawMain(int windowId)
		{
			GUI.BeginGroup(new Rect(0, /*27*/0, _windowRect.width, _windowRect.height/* - 33*/));
	
			for (int x = 0; x < _vectorUI.Count; x++) {
				_vectorUI [x].GUI ();
			}

				for (int x = 0; x < _vectorUI.Count; x++) {
					_vectorUI [x].UpdateVector ();

				}

				recalculate (_trackRide.meshGenerator, _trackRide.Track.trackSegments [_trackPosition]);
			


			GUI.EndGroup();
			GUI.DragWindow(_titleBarRect);
		}

		private void recalculate(MeshGenerator meshGenerator, TrackSegment4 segment)
		{

				foreach(Transform child in segment.gameObject.transform) {
				if (child.name != "BetweenTracksMouseCollider" && !child.name.Contains("StationPlatformTrackTile") && child.name != "MouseSelectionCollider") {
						var mesh_filter = child.gameObject.GetComponent<MeshFilter> ();
						if (mesh_filter != null) {
							Destroy (mesh_filter.mesh);
							Destroy (mesh_filter.sharedMesh);
						}
						Destroy (child.gameObject);

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
				Destroy (component.sharedMesh);
				Destroy (component.mesh);
				
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
				
			Destroy (meshCollider.sharedMesh);
				meshCollider.sharedMesh = collisionMesh;

				MouseCollider mouseCollider = segment.gameObject.GetComponent<MouseCollider>();
				mouseCollider.colliderObject = track_mouse_collider;

		}

	
	}
}

