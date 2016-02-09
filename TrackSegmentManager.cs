using System;
using System.Collections.Generic;

namespace RollercoasterEdit
{
    public class TrackSegmentManager
    {
        public TrackBuilder TrackBuilder{ get; private set; }
        public TrackedRide TrackRide{ get; private set; }


        private Dictionary<TrackSegment4,TrackSegmentModify> _trackSegments = new Dictionary<TrackSegment4, TrackSegmentModify>();

        public TrackSegmentManager (TrackBuilder trackBuilder, TrackedRide trackRide)
        {
            this.TrackBuilder = trackBuilder;
            this.TrackRide = trackRide;

            TrackRide.Track.OnAddTrackSegment += (trackSegment) => {
                _trackSegments.Add (trackSegment, new TrackSegmentModify (trackSegment, this));
            };
            TrackRide.Track.OnRemoveTrackSegment += (trackSegment) => {
                _trackSegments [trackSegment].Destroy ();
                _trackSegments.Remove (trackSegment);
            };  

        }

        public void OnDestroy()
        {
            foreach (var segment in _trackSegments.Values) {
                segment.Destroy ();
            }
        }

        public TrackSegmentModify GetTrackSegmentModifyer(TrackSegment4 segment)
        {
            return _trackSegments [segment];
        }

        public void Update()
        {
            if (_trackSegments.Count == 0) {

                for (int x = 0; x < TrackRide.Track.trackSegments.Count; x++) {
                    _trackSegments.Add (TrackRide.Track.trackSegments [x], new TrackSegmentModify (TrackRide.Track.trackSegments [x], this));

                }
            }

            foreach (var segment in _trackSegments.Values) {
                segment.Update ();
            }
        }
    }
}

