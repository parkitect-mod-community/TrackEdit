using UnityEngine;

namespace TrackEdit.Node
{
    public interface INode
    {
        
        void OnPressed(RaycastHit hit);
        void OnHold();
        void OnRelease();
        void OnNotifySegmentChange();
    }
}