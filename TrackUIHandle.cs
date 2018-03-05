using System.Reflection;
using TrackEdit.StateMachine;
using UnityEngine;

namespace TrackEdit
{
    public class TrackUiHandle : MonoBehaviour
    {
        public static TrackUiHandle Instance;
        private bool _isDirty = true;
        private FieldInfo _trackerRiderField;

        private readonly FiniteStateMachine _stateMachine = new FiniteStateMachine();
        public TrackBuilder TrackBuilder { get; private set; }
        public TrackedRide TrackRide { get; private set; }
        public TrackEditUi TrackEditUi { get; set; }

        // private GameObject TrackEditPanel;
        // private GameObject TrackBuilderPanel;


        private void Awake()
        {
            Instance = this;

            /*if (this.gameObject.GetComponent<TrackEditUI> () == null)
                trackEditUI = this.gameObject.AddComponent<TrackEditUI> ();
            else
                trackEditUI = this.gameObject.GetComponent<TrackEditUI> ();*/

            TrackBuilder = gameObject.GetComponentInChildren<TrackBuilder>();
            var flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
            _trackerRiderField = TrackBuilder.GetType().GetField("trackedRide", flags);


            //TrackBuilderPanel = this.transform.FindRecursive ("UpperModules").gameObject;


            //frame=  UIWindowsController.Instance.spawnWindow (UnityEngine.GameObject.Instantiate (Main.AssetBundleManager.UiContainerWindowGo).GetComponent<TrackEditUI>());
            //UIWindowSettings old = this.gameObject.GetComponent<UIWindowSettings> ();


            //UIWindowSettings current =  frame.gameObject.GetComponent<UIWindowSettings> ();
            //current.uniqueTag = old.uniqueTag;
        }

        private void Start()
        {
            TrackRide = (TrackedRide) _trackerRiderField.GetValue(TrackBuilder);
            _stateMachine.ChangeState(new IdleState(new SharedStateData()));

            TrackRide.Track.OnAddTrackSegment += trackSegment => { _isDirty = true; };
            TrackRide.Track.OnRemoveTrackSegment += trackSegment => { _isDirty = true; };

            Debug.Log(gameObject.GetComponent<RectTransform>().sizeDelta);
        }

        private void OnDestroy()
        {
            _stateMachine.Unload();
            for (var x = 0; x < TrackRide.Track.trackSegments.Count; x++)
                if (TrackRide.Track.trackSegments[x] != null)
                {
                    var modify = TrackRide.Track.trackSegments[x].gameObject.GetComponent<TrackSegmentModify>();
                    if (modify != null)
                        Destroy(modify);
                }
        }

        private void Update()
        {
            var ride = (TrackedRide) _trackerRiderField.GetValue(TrackBuilder);
            if (ride != TrackRide)
            {
                Destroy(this);
                gameObject.AddComponent<TrackUiHandle>();
            }

            if (_isDirty)
            {
                for (var x = 0; x < TrackRide.Track.trackSegments.Count; x++)
                    if (!TrackRide.Track.trackSegments[x].gameObject.GetComponent<TrackSegmentModify>())
                        TrackRide.Track.trackSegments[x].gameObject.AddComponent<TrackSegmentModify>();
                _isDirty = false;
            }

            //_trackSegmentManger.Update ();
            _stateMachine.Update();
        }
    }
}