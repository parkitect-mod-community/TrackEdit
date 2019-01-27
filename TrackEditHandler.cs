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

        private readonly Dictionary<TrackSegment4,TrackSegmentHandler> _handlers = new Dictionary<TrackSegment4, TrackSegmentHandler>();

        public TrackSegmentHandler GetSegmentHandler(TrackSegment4 segment)
        {
            if(_handlers.ContainsKey(segment))
                return _handlers[segment];
            return null;
        }

        private void Awake()
        {
            TrackBuilder = gameObject.GetComponentInChildren<TrackBuilder>();
            var flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
            _trackerRiderField = TrackBuilder.GetType().GetField("trackedRide", flags);

        }

        private void Start()
        {
            WithTrackedRideChange();
        }

        private void refreshHandlers()
        {
            foreach (var segment in TrackRide.Track.trackSegments)
            {
                if (!_handlers.ContainsKey(segment))
                {
                    RegisterHandler(segment);
                }
            }

            foreach (var segment in TrackRide.Track.trackSegments)
            {
                TrackSegmentHandler handler = GetSegmentHandler(segment);
                if(handler  != null)
                    handler.NotifySegmentChange();

            }
        }

        public TrackSegmentHandler RegisterHandler(TrackSegment4 segment)
        {
            segment.OnDestroyed += () => {
                if (_handlers.ContainsKey(segment)){
                    TrackSegmentHandler handler = _handlers[segment];
                            
                    TrackSegmentHandler previous = handler.GetPreviousSegment(true);
                    TrackSegmentHandler next = handler.GetNextSegment(true);
                            
                    if(previous != null)
                        previous.NotifySegmentChange();
                    if(next != null)
                        next.NotifySegmentChange();

                    _handlers.Remove(segment);
                    handler.OnDestroy();
                }
            };
            TrackSegmentHandler h = new TrackSegmentHandler(segment, this);
            _handlers.Add(segment, h);
            return h;
        }

        private void OnDestroy()
        {
            clearHandlers();
        }

        private void clearHandlers()
        {
            foreach (var handler in _handlers.Values)
            {
                handler.OnDestroy();
            }   
            _handlers.Clear();
        }

        private void WithTrackedRideChange()
        {
            TrackRide = (TrackedRide) _trackerRiderField.GetValue(TrackBuilder);
            clearHandlers();
            TrackRide.Track.OnAddTrackSegment += trackSegment =>  refreshHandlers();
            TrackRide.Track.OnRemoveTrackSegment += trackSegment => refreshHandlers();    
            
            refreshHandlers();
        }


        private void Update()
        {
            var ride = (TrackedRide) _trackerRiderField.GetValue(TrackBuilder);
            if (ride != TrackRide)
            {
                clearHandlers();
            }

            foreach (var segment in TrackRide.Track.trackSegments)
            {
                TrackSegmentHandler handler = GetSegmentHandler(segment);
                if (handler != null)
                    handler.Update();

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