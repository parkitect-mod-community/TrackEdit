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

namespace TrackEdit
{
    public class RotationNode : MonoBehaviour, INode
    {
        public TrackNode AttachedNode { get; set; }

        private void Start()
        {
            AttachedNode = transform.parent.parent.GetComponent<TrackNode>();
            transform.parent.gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.parent.position = AttachedNode.transform.position;
            transform.parent.rotation =
                Quaternion.LookRotation(AttachedNode.TrackSegmentModify.TrackSegment.getTangentPoint(1f));

            transform.localEulerAngles = new Vector3(0, 0, AttachedNode.TrackSegmentModify.TrackSegment.totalRotation);
            transform.parent.Find("Angle").GetComponent<TextMesh>().text =
                AttachedNode.TrackSegmentModify.TrackSegment.totalRotation % 360 + "\u00B0";
            transform.parent.Find("Angle").LookAt(Camera.main.transform, Vector3.up);
        }
    }
}