using System;
using System.Collections.Generic;
using UnityEngine;

namespace RollercoasterEdit
{
    public class TrackSegmentManager
    {
        public TrackBuilder TrackBuilder{ get; private set; }
        public TrackedRide TrackRide{ get; private set; }
		private Dictionary<string,TrackSegmentModify> _trackSegments = new Dictionary<string, TrackSegmentModify>();

        public TrackSegmentManager (TrackBuilder trackBuilder, TrackedRide trackRide)
        {
            this.TrackBuilder = trackBuilder;
            this.TrackRide = trackRide;

            TrackRide.Track.OnAddTrackSegment += (trackSegment) => {
				foreach (var segment in _trackSegments.Values) {
					segment.Destroy ();
				}
				_trackSegments.Clear();
				//_trackSegments.Add (trackSegment.getId(), new TrackSegmentModify (trackSegment, this));
            };
			TrackRide.Track.OnRemoveTrackSegment += (trackSegment) => {
				foreach (var segment in _trackSegments.Values) {
					segment.Destroy ();
				}
				_trackSegments.Clear();
			//	_trackSegments [trackSegment.getId()].Destroy ();
				//_trackSegments.Remove (trackSegment.getId());
            };  

        }

		public void ConnectEndPieces(TrackSegmentModify previous,TrackSegmentModify next)
		{
			next.TrackSegment.isConnectedToNextSegment = true;
			previous.TrackSegment.isConnectedToNextSegment = true;

			next.TrackSegment.isConnectedToPreviousSegment = true;
			previous.TrackSegment.isConnectedToPreviousSegment = true;

		
			float magnitude = Mathf.Abs((next.GetFirstCurve.P0.GetGlobal () - next.GetFirstCurve.P1.GetGlobal ()).magnitude);

			previous.GetLastCurve.P2.SetPoint(previous.GetLastCurve.P3.GetGlobal() + (next.TrackSegment.getTangentPoint(0f) *-1f* magnitude));
			previous.GetLastCurve.P2.Validate ();

			previous.Invalidate = true;
		}


        public void OnDestroy()
        {
            foreach (var segment in _trackSegments.Values) {
                segment.Destroy ();
            }
        }

        public TrackSegmentModify GetTrackSegmentModifyer(TrackSegment4 segment)
        {
			return _trackSegments [segment.getId()];
        }

		public TrackSegmentModify GetLastSegment()
		{
			return GetTrackSegmentModifyer (TrackRide.Track.trackSegments [TrackRide.Track.trackSegments.Count - 1]);
		}

		public TrackSegmentModify GetFirstSegment()
		{
			return GetTrackSegmentModifyer (TrackRide.Track.trackSegments [0]);

		}

        public void Update()
        {
            if (_trackSegments.Count == 0) {

                for (int x = 0; x < TrackRide.Track.trackSegments.Count; x++) {
					_trackSegments.Add (TrackRide.Track.trackSegments [x].getId(), new TrackSegmentModify (TrackRide.Track.trackSegments [x], this));

                }
				foreach (var segment in _trackSegments.Values) {
					segment.Load ();
				}
            }

            foreach (var segment in _trackSegments.Values) {
                segment.Update ();
            }
        }
    }
}

