using UnityEngine;

namespace TrackEdit.Node
{
    public abstract class BaseNode : MonoBehaviour, INode
    {
        public delegate void OnPositionChange(BaseNode node);

        public delegate void OnReleaseNode(BaseNode node);
        public event OnPositionChange OnDragEvent;
        public event OnReleaseNode OnReleaseEvent;
 

        public abstract void OnPressed(RaycastHit hit);
        
        public virtual void OnHold()
        {
            if (OnDragEvent != null) OnDragEvent.Invoke(this);
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