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

namespace TrackEdit.Node
{
    public abstract class BaseNode : MonoBehaviour, INode
    {
        public delegate void OnNode(BaseNode node);

        public event OnNode OnHoldEvent;
        public event OnNode OnReleaseEvent;
        public event OnNode OnBeginHoldEvent;


        public virtual void OnBeginHold(RaycastHit hit)
        {
            if (OnBeginHoldEvent != null) OnBeginHoldEvent.Invoke(this);
        }

        public virtual void OnHold()
        {
            if (OnHoldEvent != null) OnHoldEvent.Invoke(this);
        }

        public virtual void OnRelease()
        {
            if (OnReleaseEvent != null) OnReleaseEvent.Invoke(this);
        }

        public abstract void OnNotifySegmentChange();
        protected abstract void Update();
        protected abstract void Awake();

    }
}