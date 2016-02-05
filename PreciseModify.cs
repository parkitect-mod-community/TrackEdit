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
		private TrackSegment4 _segment;

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
				_segment = ((TrackedRide)_trackerRiderField.GetValue (_trackBuilder)).Track.trackSegments [_trackPosition];
				for (int x = 0; x < _segment.curves.Count; x++) {
					_vectorUI.Add (new HelloMod.VectorUI(_segment.curves [x]));
				}
			}

		}
			
		void OnGUI()
		{
			if (_segment != null)
			_windowRect = GUILayout.Window(45051 , _windowRect, DrawMain, "Bezire Modifer");

		}
		public void DrawMain(int windowId)
		{
			GUI.BeginGroup(new Rect(0, /*27*/0, _windowRect.width, _windowRect.height/* - 33*/));
	
			for (int x = 0; x < _vectorUI.Count; x++) {
				_vectorUI [x].GUI ();
			}

			GUI.EndGroup();
			GUI.DragWindow(_titleBarRect);
		}

		private Vector3 VectorUI(Vector3 vector)
		{
			string vectorx = vector.x+"";
			string vectory = vector.y+"";
			string vectorz = vector.z+"";

			GUILayout.BeginHorizontal ();
			vectorx=GUILayout.TextField (vectorx);
			vectory=GUILayout.TextField (vectory);
			vectorz=GUILayout.TextField (vectorz);
			GUILayout.EndHorizontal ();

			float result;
			Vector3 output = new Vector3 ();
			if (float.TryParse (vectorx, out result)) {
				output.x = result;
			}
			if (float.TryParse (vectory, out result)) {
				output.y = result;
			}
			if (float.TryParse (vectorz, out result)) {
				output.z = result;
			}
			return output;

		}
	}
}

