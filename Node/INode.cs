using UnityEngine;

namespace TrackEdit.Node
{
    public interface INode
    {
        
        void OnBeginHold(RaycastHit hit);
        void OnHold();
        void OnRelease();
        void OnNotifySegmentChange();
    }
}