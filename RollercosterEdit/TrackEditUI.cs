using System;
using Parkitect.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RollercoasterEdit
{
    public class TrackEditUI : MonoBehaviour
    {

        public TrackUIHandle trackUIHandle;

        public Toggle chainToggle;

        public Toggle TrackBuilderToggle;
        public Toggle TrackEditToggle;

        public Transform TrackBuilderPanel;
        public Transform TrackEditPanel;

        public Button LeftTrackButton;
        public Button RightTrackButton;
        public Button DestroyTrackButton;


        void Start()
        {
            trackUIHandle = this.GetComponent<TrackUIHandle> ();

            LeftTrackButton = this.transform.FindRecursive ("LeftTrack").GetComponent<Button> ();
            RightTrackButton = this.transform.FindRecursive ("RightTrack").GetComponent<Button> ();
            DestroyTrackButton = this.transform.FindRecursive ("DestroyTrack").GetComponent<Button> ();

            TrackBuilderPanel = this.transform.FindRecursive ("UpperModules");
            TrackEditPanel = this.transform.FindRecursive ("TrackEditPanel");

            chainToggle = this.transform.FindRecursive ("ChainBuilderToggle").GetComponent<Toggle> ();

            TrackBuilderToggle = this.transform.FindRecursive ("TrackBuilderToggle").GetComponent<Toggle> ();
            TrackEditToggle = this.transform.FindRecursive ("TrackEditToggle").GetComponent<Toggle> ();


            TrackBuilderToggle.onValueChanged.AddListener((delegate(bool arg0) {
                TrackBuilderPanel.gameObject.SetActive(arg0);
            }));

            TrackEditToggle.onValueChanged.AddListener((delegate(bool arg0) {
                TrackEditPanel.gameObject.SetActive(arg0);
            }));

            LeftTrackButton.onClick.AddListener (new UnityEngine.Events.UnityAction (delegate {
                trackUIHandle.trackBuilder.moveTrackCursorPosition(-1);

            }));

            RightTrackButton.onClick.AddListener (new UnityEngine.Events.UnityAction (delegate {
                trackUIHandle.trackBuilder.moveTrackCursorPosition(1);
            }));

            DestroyTrackButton.onClick.AddListener (new UnityEngine.Events.UnityAction (delegate {
                trackUIHandle.trackBuilder.remove();
                trackUIHandle.trackBuilder.moveTrackCursorPosition(-1);
            }));

            TrackEditPanel.gameObject.SetActive (false);


        }
    }
}

