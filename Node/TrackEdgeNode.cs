using System.Linq;
using UnityEngine;

namespace TrackEdit.Node
{
    public class TrackEdgeNode: EmptyNode, IActivatable
    {

        public TrackSegmentHandler Forward { get; set; }
        public TrackSegmentHandler Current { get; set; }

        private bool _isActive = false;

        private TextMesh _text;
        
        private RotationNode _rotationNode;
        private EmptyNode _forwardNode;
        private EmptyNode _backwardNode;
        
        

        public override void Awake()
        {
            base.Awake();
            _forwardNode = Build<EmptyNode>();
            _backwardNode = Build<EmptyNode>();
            _rotationNode = RotationNode.build(Current);

            
            _rotationNode.transform.parent = transform;
            _rotationNode.transform.position = Vector3.zero;

        }

        public override void Update()
        {
            Vector3 pos = _forwardNode.transform.TransformPoint(Vector3.zero);
            base.Update();
        }

        public void onActivate(RaycastHit hit)
        {
            _isActive = true;
        }

        public void onDeactivate()
        {
            _isActive = false;
        }

        public override void OnHold()
        {
            base.OnHold();
        }

        public override void OnRelease()
        {
            base.OnRelease();
        }

 
      
    }
}