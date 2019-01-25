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

//        public void SetActiveNode(Transform active)
//        {
//            if (active != _activeNode)
//            {
//                if (_activeNode != null)
//                    _activeNode.GetComponent<TrackNode>().ActivateNeighbors(false);
//                if (active != null)
//                    active.GetComponent<TrackNode>().ActivateNeighbors(true);
//            }
//
//            _activeNode = active;
//        }


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
                WithTrackedRideChange();
            }

            foreach (var segment in TrackRide.Track.trackSegments)
            {
                TrackSegmentHandler handler = GetSegmentHandler(segment);
                if(handler  != null)
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
                            Debug.Log(hit.transform.gameObject.name);
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

                            if(_hold == null)
                                node.OnPressed(hit);
                            _hold = node;
                        }
                  
                        
                    
                }

                if (_hold != null)
                {
                    Debug.Log("holding");
                    _hold.OnHold();
                }

                if (Input.GetMouseButtonUp(0))
                {
                    Debug.Log("Release");
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

//        private TrackEditState HandleState()
//        {
//            switch (_trackEditState)
//            {
//                case TrackEditState.Idle:
//                    return HandleIdleState();
//                case TrackEditState.Drag:
//                    return HandleDragState();
//                case TrackEditState.Freedrag:
//                    return HandleFreeDragState();
//                case TrackEditState.FreeDragWithSmoothing:
//                    return HandleFreeDragSmoothingState();
//                case TrackEditState.LinearDrag:
//                    return HandleLinearDragState();
//                case TrackEditState.Rotate:
//                    return HandleRotateState();
//            }
//
//            return TrackEditState.Idle;
//        }
//    
//      
//        private TrackEditState HandleIdleState()
//        {
//            Camera cam = Camera.main;
//            if (!cam) return TrackEditState.Idle;
//            
//            var ray = cam.ScreenPointToRay(Input.mousePosition);
//            RaycastHit hit;
//            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMasks.ID_COASTER_TRACKS))
//            {
//                
//                var position = hit.transform.position;
//                
//                _selected = hit.transform;
//                _fixedY = position.y;
//                _offset = new Vector3(position.x - hit.point.x, 0,position.z - hit.point.z);
//
//                float distance = (ray.origin - hit.point).magnitude;
//
//                if (hit.transform.name == "BezierNode")
//                {
//                    SetActiveNode(hit.transform);
//
//                    var node = hit.transform.GetComponent<TrackNode>();
//
//                    var nextSegment = node.TrackSegmentModify.GetNextSegment(true);
//                    var previousSegment = node.TrackSegmentModify.GetPreviousSegment(true);
//
//                    if (node.NodePoint == TrackNode.NodeType.P1 && previousSegment != null &&
//                        previousSegment.TrackSegment is Station)
//                    {
//                        return TrackEditState.LinearDrag;
//                    }
//                    else if (node.NodePoint == TrackNode.NodeType.P2 && nextSegment != null &&
//                             nextSegment.TrackSegment is Station)
//                    {
//                        return TrackEditState.LinearDrag;
//                    }
//                    else
//                    {
//                        
//                        return TrackEditState.Freedrag;
//                    }
//                
//                }
//                else if (hit.transform.name == "ExtrudeNode")
//                {
////                    stateMachine.ChangeState(new ConsumeExtrudeNodeState(_stateData));
//                }
//                else if (hit.transform.name == "Rotate")
//                {
////                    stateMachine.ChangeState(new RotationState(_stateData));
//                }
//            }
//
//            return TrackEditState.Idle;
//        }
//
//        private TrackEditState HandleDragState()
//        {
//            return TrackEditState.Idle;
//        }
//
//        private TrackEditState HandleFreeDragState()
//        {
//            return TrackEditState.Idle;
//        }
//
//        private TrackEditState HandleFreeDragSmoothingState()
//        {
//            return TrackEditState.Idle;
//        }
//
//        private TrackEditState HandleLinearDragState()
//        {
//            return TrackEditState.Idle;
//        }
//
//        private TrackEditState HandleRotateState()
//        {
//            return TrackEditState.Idle;
//        }

    }
}