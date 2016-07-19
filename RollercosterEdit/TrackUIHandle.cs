using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Parkitect.UI;
using UnityEngine.UI;

namespace RollercoasterEdit
{
	public class TrackUIHandle : MonoBehaviour
	{
		public TrackBuilder trackBuilder{ get; private set; }
		public TrackedRide trackRide{ get; private set; }
        private FieldInfo trackerRiderField;

        private FiniteStateMachine stateMachine = new FiniteStateMachine ();

        public static TrackUIHandle instance = null;
        public TrackEditUI trackEditUI { get; set; }
        private bool isDirty = true;

       // private GameObject TrackEditPanel;
       // private GameObject TrackBuilderPanel;

  
		void Awake()
		{
            TrackUIHandle.instance = this;

            trackEditUI = this.gameObject.AddComponent<TrackEditUI> ();

            trackBuilder = this.gameObject.GetComponentInChildren<TrackBuilder>();
            BindingFlags flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
			trackerRiderField = trackBuilder.GetType ().GetField ("trackedRide", flags);
           


            //TrackBuilderPanel = this.transform.FindRecursive ("UpperModules").gameObject;



            //frame=  UIWindowsController.Instance.spawnWindow (UnityEngine.GameObject.Instantiate (Main.AssetBundleManager.UiContainerWindowGo).GetComponent<TrackEditUI>());
            //UIWindowSettings old = this.gameObject.GetComponent<UIWindowSettings> ();


            //UIWindowSettings current =  frame.gameObject.GetComponent<UIWindowSettings> ();
            //current.uniqueTag = old.uniqueTag;
           
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
            TrackedRide ride = ((TrackedRide)trackerRiderField.GetValue (trackBuilder));
            if (ride != trackRide) {
                UnityEngine.Object.Destroy (this);
                this.gameObject.AddComponent<TrackUIHandle> ();
            }

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

