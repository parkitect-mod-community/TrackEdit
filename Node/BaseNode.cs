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