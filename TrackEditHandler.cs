using System.Reflection;
using TrackEdit.StateMachine;
using UnityEngine;

namespace TrackEdit
{
    public class TrackEditHandler : MonoBehaviour
    {
        public enum TrackEditState
        {
            Idle,
            Drag,
            Freedrag,
            FreeDragWithSmoothing,
            LinearDrag,
            Rotate

        }

        public static TrackEditHandler Instance;
        private bool _isDirty = true;
        private FieldInfo _trackerRiderField;

        public float _distance;

        public float _fixedY;
        public Vector3 _offset;

        public Transform _selected;
        public Transform _activeNode;

        public TrackEditState _trackEditState = TrackEditState.Idle;

        public TrackBuilder TrackBuilder { get; private set; }
        public TrackedRide TrackRide { get; private set; }



        public void SetActiveNode(Transform active)
        {
            if (active != _activeNode)
            {
                if (_activeNode != null)
                    _activeNode.GetComponent<TrackNode>().ActivateNeighbors(false);
                if (active != null)
                    active.GetComponent<TrackNode>().ActivateNeighbors(true);
            }

            _activeNode = active;
        }


        private void Awake()
        {
            Instance = this;

            TrackBuilder = gameObject.GetComponentInChildren<TrackBuilder>();
            var flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
            _trackerRiderField = TrackBuilder.GetType().GetField("trackedRide", flags);

        }

        private void Start()
        {
            TrackRide = (TrackedRide) _trackerRiderField.GetValue(TrackBuilder);

            TrackRide.Track.OnAddTrackSegment += trackSegment => { _isDirty = true; };
            TrackRide.Track.OnRemoveTrackSegment += trackSegment => { _isDirty = true; };

            Debug.Log(gameObject.GetComponent<RectTransform>().sizeDelta);
        }

        private void OnDestroy()
        {
            for (var x = 0; x < TrackRide.Track.trackSegments.Count; x++)
            {
                if (TrackRide.Track.trackSegments[x] != null)
                {
                    var modify = TrackRide.Track.trackSegments[x].gameObject.GetComponent<TrackSegmentModify>();
                    if (modify != null)
                        Destroy(modify);
                }
            }
        }

        private void Update()
        {
            var ride = (TrackedRide) _trackerRiderField.GetValue(TrackBuilder);
            if (ride != TrackRide)
            {
                Destroy(this);
                gameObject.AddComponent<TrackEditHandler>();
            }

            if (_isDirty)
            {
                for (var x = 0; x < TrackRide.Track.trackSegments.Count; x++)
                    if (!TrackRide.Track.trackSegments[x].gameObject.GetComponent<TrackSegmentModify>())
                        TrackRide.Track.trackSegments[x].gameObject.AddComponent<TrackSegmentModify>();
                _isDirty = false;
            }

            _trackEditState = HandleState();

        }

        private TrackEditState HandleState()
        {
            switch (_trackEditState)
            {
                case TrackEditState.Idle:
                    return HandleIdleState();
                case TrackEditState.Drag:
                    return HandleDragState();
                case TrackEditState.Freedrag:
                    return HandleFreeDragState();
                case TrackEditState.FreeDragWithSmoothing:
                    return HandleFreeDragSmoothingState();
                case TrackEditState.LinearDrag:
                    return HandleLinearDragState();
                case TrackEditState.Rotate:
                    return HandleRotateState();
            }

            return TrackEditState.Idle;
        }

        private TrackEditState HandleIdleState()
        {
            return TrackEditState.Idle;
        }

        private TrackEditState HandleDragState()
        {
            return TrackEditState.Idle;
        }

        private TrackEditState HandleFreeDragState()
        {
            return TrackEditState.Idle;
        }

        private TrackEditState HandleFreeDragSmoothingState()
        {
            return TrackEditState.Idle;
        }

        private TrackEditState HandleLinearDragState()
        {
            return TrackEditState.Idle;
        }

        private TrackEditState HandleRotateState()
        {
            return TrackEditState.Idle;
        }

    }
}