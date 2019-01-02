/**
* Copyright 2019 Michael Pollind
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using UnityEngine;
using UnityEngine.UI;

namespace TrackEdit
{
    public class TrackEditUi : MonoBehaviour
    {
        public Toggle ChainToggle;
        public Button DestroyTrackButton;

        public Button LeftTrackButton;
        public Button RightTrackButton;

        public Transform TrackBuilderPanel;

        public Toggle TrackBuilderToggle;
        public Transform TrackEditPanel;
        public Toggle TrackEditToggle;

        public TrackUiHandle TrackUiHandle;


        private void Start()
        {
            TrackUiHandle = GetComponent<TrackUiHandle>();

            LeftTrackButton = transform.FindRecursive("LeftTrack").GetComponent<Button>();
            RightTrackButton = transform.FindRecursive("RightTrack").GetComponent<Button>();
            DestroyTrackButton = transform.FindRecursive("DestroyTrack").GetComponent<Button>();

            TrackBuilderPanel = transform.FindRecursive("UpperModules");
            TrackEditPanel = transform.FindRecursive("TrackEditPanel");

            ChainToggle = transform.FindRecursive("ChainBuilderToggle").GetComponent<Toggle>();

            TrackBuilderToggle = transform.FindRecursive("TrackBuilderToggle").GetComponent<Toggle>();
            TrackEditToggle = transform.FindRecursive("TrackEditToggle").GetComponent<Toggle>();


            TrackBuilderToggle.onValueChanged.AddListener(delegate(bool arg0)
            {
                TrackBuilderPanel.gameObject.SetActive(arg0);
            });

            TrackEditToggle.onValueChanged.AddListener(delegate(bool arg0)
            {
                TrackEditPanel.gameObject.SetActive(arg0);
            });

            LeftTrackButton.onClick.AddListener(delegate { TrackUiHandle.TrackBuilder.moveTrackCursorPosition(-1); });

            RightTrackButton.onClick.AddListener(delegate { TrackUiHandle.TrackBuilder.moveTrackCursorPosition(1); });

            DestroyTrackButton.onClick.AddListener(delegate
            {
                TrackUiHandle.TrackBuilder.remove();
                TrackUiHandle.TrackBuilder.moveTrackCursorPosition(-1);
            });

            TrackEditPanel.gameObject.SetActive(false);
        }
    }
}