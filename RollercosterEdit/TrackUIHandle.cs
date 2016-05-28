using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace RollercoasterEdit
{
	public class TrackUIHandle : MonoBehaviour
	{
		public TrackBuilder TrackBuilder{ get; private set; }
		public TrackedRide TrackRide{ get; private set; }
        private FieldInfo _trackerRiderField;

        private FiniteStateMachine _stateMachine = new FiniteStateMachine ();

        public static TrackUIHandle instance = null;
        private bool _isDirty = true;


		void Awake()
		{
            TrackUIHandle.instance = this;

            TrackBuilder = this.gameObject.GetComponent<TrackBuilder>();
            BindingFlags flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
			_trackerRiderField = TrackBuilder.GetType ().GetField ("trackedRide", flags);
		}

		void Start() {
			TrackRide = ((TrackedRide)_trackerRiderField.GetValue (TrackBuilder));
			TrackBuilder = this.gameObject.GetComponent<TrackBuilder>();
           // _trackSegmentManger = new TrackSegmentManager (TrackBuilder, TrackRide);
			
            var sharedStateData = new SharedStateData ();
			//sharedStateData.SegmentManager = _trackSegmentManger;
			_stateMachine.ChangeState (new IdleState (sharedStateData));

            TrackRide.Track.OnAddTrackSegment += (trackSegment) => {
                _isDirty = true;
            };
            TrackRide.Track.OnRemoveTrackSegment += (trackSegment) => {
                _isDirty = true;
            };  


		}

		void OnDestroy() {
            for (int x = 0; x < TrackRide.Track.trackSegments.Count; x++) {
                Destroy(TrackRide.Track.trackSegments [x].gameObject.GetComponent<TrackSegmentModify> ());
            }
            //_trackSegmentManger.OnDestroy ();
		}

		void Update()
		{
            if (_isDirty) {
                for (int x = 0; x <  TrackRide.Track.trackSegments.Count; x++) {
                    if (!TrackRide.Track.trackSegments [x].gameObject.GetComponent<TrackSegmentModify> ()) {
                        TrackRide.Track.trackSegments [x].gameObject.AddComponent<TrackSegmentModify> ();
                    }
                }
                _isDirty = false;
            }

			//_trackSegmentManger.Update ();
			_stateMachine.Update ();

		}


	
	}
}

