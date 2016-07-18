using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Parkitect.UI;

namespace RollercoasterEdit
{
	public class TrackUIHandle : MonoBehaviour
	{
		public TrackBuilder trackBuilder{ get; private set; }
		public TrackedRide trackRide{ get; private set; }
        private FieldInfo trackerRiderField;

        private FiniteStateMachine stateMachine = new FiniteStateMachine ();

        public static TrackUIHandle instance = null;
        private bool isDirty = true;


		void Awake()
		{
            TrackUIHandle.instance = this;

            trackBuilder = this.gameObject.GetComponent<TrackBuilder>();
            BindingFlags flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
			trackerRiderField = trackBuilder.GetType ().GetField ("trackedRide", flags);
           
            /*UIWindowFrame frame =  UIWindowsController.Instance.spawnWindow (UnityEngine.GameObject.Instantiate (Main.AssetBundleManager.UiWindowGo).GetComponent<TrackEditUI>());
            frame.onClose += (UIWindowFrame windowFrame) => {
                this.GetComponent<UIWindowFrame>().close();
            };*/
        }

		void Start() {
			trackRide = ((TrackedRide)trackerRiderField.GetValue (trackBuilder));
			trackBuilder = this.gameObject.GetComponent<TrackBuilder>();
           // _trackSegmentManger = new TrackSegmentManager (TrackBuilder, TrackRide);
			
            var sharedStateData = new SharedStateData ();
			//sharedStateData.SegmentManager = _trackSegmentManger;
			stateMachine.ChangeState (new IdleState (sharedStateData));

            trackRide.Track.OnAddTrackSegment += (trackSegment) => {
                isDirty = true;
            };
            trackRide.Track.OnRemoveTrackSegment += (trackSegment) => {
                isDirty = true;
            };  


		}

		void OnDestroy() {
            stateMachine.Unload ();
            for (int x = 0; x < trackRide.Track.trackSegments.Count; x++) {
                Destroy(trackRide.Track.trackSegments [x].gameObject.GetComponent<TrackSegmentModify> ());
            }
		}

		void Update()
		{
            if (isDirty) {
                for (int x = 0; x <  trackRide.Track.trackSegments.Count; x++) {
                    if (!trackRide.Track.trackSegments [x].gameObject.GetComponent<TrackSegmentModify> ()) {
                        trackRide.Track.trackSegments [x].gameObject.AddComponent<TrackSegmentModify> ();
                    }
                }
                isDirty = false;
            }

			//_trackSegmentManger.Update ();
			stateMachine.Update ();

		}


	
	}
}

