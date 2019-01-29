using System.Collections.Generic;
using System.Reflection;
using TrackEdit.Node;
using UnityEngine;

namespace TrackEdit
{
    public class TrackEditHandler : MonoBehaviour
    {
        public TrackBuilder TrackBuilder { get; private set; }
        public TrackedRide TrackRide { get; private set; }

        private FieldInfo _trackerRiderField;
        private IActivatable _activatedNode = null;
        private INode _hold = null;

        private void Awake()
        {
            TrackBuilder = gameObject.GetComponentInChildren<TrackBuilder>();
            var flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
            _trackerRiderField = TrackBuilder.GetType().GetField("trackedRide", flags);

        }

        private void Start()
        {
            TrackRide = (TrackedRide) _trackerRiderField.GetValue(TrackBuilder);
            clearHandlers();
            TrackRide.Track.OnAddTrackSegment += trackSegment =>
            {
                Debug.Log("On Track Added");
                TrackRide.Track.recalculateIsClosedTrack();
                refreshHandlers();
            };
            TrackRide.Track.OnRemoveTrackSegment += trackSegment =>
            {
                Debug.Log("On Track Removed");
                TrackRide.Track.recalculateIsClosedTrack();
                clearHandlers();
                refreshHandlers();
            };    
            refreshHandlers();
        }

        private void refreshHandlers()
        {
            foreach (var segment in TrackRide.Track.trackSegments)
            {
                GameObject ob = segment.gameObject;
                TrackSegmentHandler handler = ob.GetComponent<TrackSegmentHandler>();
                if (handler == null)
                    handler = ob.AddComponent<TrackSegmentHandler>();
                handler.Handler = this;
            }
            
            foreach (var segment in TrackRide.Track.trackSegments)
            {
                GameObject ob = segment.gameObject;
                TrackSegmentHandler handler = ob.GetComponent<TrackSegmentHandler>();
                
                if (handler != null)
                {
                    handler.NotifySegmentChange();
                }
            }
        }


        private void OnDestroy()
        {
            clearHandlers();
        }

        private void clearHandlers()
        {
            foreach (var segment in TrackRide.Track.trackSegments)
            {
                Object.Destroy(segment.gameObject.GetComponent<TrackSegmentHandler>());           
            }
        }

        private void Update()
        {
            var ride = (TrackedRide) _trackerRiderField.GetValue(TrackBuilder);
            if (ride != TrackRide)
            {
                clearHandlers();
                TrackRide = ride;
            }

            Camera cam = Camera.main;
            if (cam != null)
            {
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                if (Input.GetMouseButtonDown(0))
                {

                    if (Physics.Raycast(ray, out var hit, Mathf.Infinity, LayerMasks.ID_COASTER_TRACKS))
                    {
                        INode node = hit.transform.gameObject.GetComponent<BaseNode>();
                        if (node is IActivatable)
                        {
                            IActivatable activate = (IActivatable) node;
                            if (activate != _activatedNode)
                            {
                                if (_activatedNode != null)
                                {
                                    _activatedNode.onDeactivate();
                                }

                                activate.onActivate(hit);
                                _activatedNode = activate;
                            }
                        }

                        if (_hold == null)
                        {
                            node.OnBeginHold(hit);
                        }

                        _hold = node;
                    }

                }

                if (_hold != null)
                {
                   
                    _hold.OnHold();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (_hold != null)
                    {
                        _hold.OnRelease();
                        _hold = null;
                    }
                }

            }
        }

        // TODO: allow for more then one node to be activated
        public IActivatable[] GetActivatedNodes()
        {
            return new[] {_activatedNode};
        }


    }
}