using UnityEngine;

namespace TrackEdit.Node
{
    public interface IActivatable
    {
        void onActivate(RaycastHit hit);
        void onDeactivate();
    }
}