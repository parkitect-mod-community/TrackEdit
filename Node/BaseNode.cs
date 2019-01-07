using UnityEngine;

namespace TrackEdit.Node
{
    public abstract class BaseNode : MonoBehaviour, INode
    {
        public delegate string OnPositionChange(EmptyNode node);

        public abstract void OnPressed(RaycastHit hit);

        public abstract void OnHold();
        public abstract void OnRelease();
    }
}