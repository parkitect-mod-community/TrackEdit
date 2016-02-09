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

        private TrackSegmentManager _trackSegmentManger;
        private FiniteStateMachine _stateMachine = new FiniteStateMachine ();

		public TrackUIHandle ()
		{
			
		}

		void Awake()
		{
			TrackBuilder = this.gameObject.GetComponent<TrackBuilder>();
			BindingFlags flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
			_trackerRiderField = TrackBuilder.GetType ().GetField ("trackedRide", flags);
		}
		void Start() {
			TrackRide = ((TrackedRide)_trackerRiderField.GetValue (TrackBuilder));
			TrackBuilder = this.gameObject.GetComponent<TrackBuilder>();
            _trackSegmentManger = new TrackSegmentManager (TrackBuilder, TrackRide);
            _stateMachine.ChangeState (new IdleState ());
		}

		void OnDestroy() {
            _trackSegmentManger.OnDestroy ();
		}
		void Update()
		{
            _trackSegmentManger.Update ();

            _stateMachine.Update ();

		}
	
	}
}

