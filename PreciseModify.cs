using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace HelloMod
{
	public class PreciseModify : MonoBehaviour
	{
		private FieldInfo _trackerRiderField;
		private TrackBuilder _trackBuilder;

		public TrackedRide _trackRide;

		private Dictionary<TrackSegment4,CubeBezierModify> _segments = new Dictionary<TrackSegment4,CubeBezierModify> ();

		private Transform _selected;
		private Vector3 _offset;
		private float _dist;



		public PreciseModify ()
		{
			
		}

		void Awake()
		{
			_trackBuilder = this.gameObject.GetComponent<TrackBuilder>();
			BindingFlags flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
			_trackerRiderField = _trackBuilder.GetType ().GetField ("trackedRide", flags);

		

		
		}
		void Start() {


			_trackRide = ((TrackedRide)_trackerRiderField.GetValue (_trackBuilder));

		
			_trackRide.Track.OnAddTrackSegment += (trackSegment) => {
				_segments.Add (trackSegment, new CubeBezierModify (trackSegment, this));
			};
			_trackRide.Track.OnRemoveTrackSegment += (trackSegment) => {
				_segments [trackSegment].Destroy ();
				_segments.Remove (trackSegment);
			};	
		

		}
		void Update()
		{
			if (_segments.Count == 0) {

				_trackRide = ((TrackedRide)_trackerRiderField.GetValue (_trackBuilder));

				for (int x = 0; x < _trackRide.Track.trackSegments.Count; x++) {
					_segments.Add (_trackRide.Track.trackSegments [x], new CubeBezierModify (_trackRide.Track.trackSegments [x], this));

				}
			}
	
			var ray = Camera.main.ScreenPointToRay (Input.mousePosition);

			if (Input.GetMouseButtonDown (0)) {
				if (!_selected) {
					RaycastHit hit;
					if (Physics.Raycast (ray, out hit, Mathf.Infinity,-1)) {
						if (hit.transform.name == "BezierNode") {
							_selected = hit.transform;
							_offset = hit.transform.position - hit.point;
							_dist = (ray.origin - hit.point).magnitude;

						}
					}
				}
			} else if (Input.GetMouseButtonUp (0)) 
			{
				_selected = null;
			}

			if (_selected) {
				_selected.gameObject.GetComponent<Node> ().NodeUpdate ();

				_selected.position = ray.GetPoint (_dist) + _offset;
			}





		}
	
	}
}

