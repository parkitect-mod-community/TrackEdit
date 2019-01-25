using System.Linq;
using UnityEngine;

namespace TrackEdit.Node
{
    public class TrackEdgeNode: EmptyNode, IActivatable
    {

        public TrackSegmentHandler Forward { get; set; }
        public TrackSegmentHandler Current { get; set; }

       // private bool _isActive = false;

        //private RotationNode _rotationNode;
        private EmptyNode _forwardNode;
        private EmptyNode _backwardNode;



        protected override void Awake()
        {
            base.Awake();
            _forwardNode = Build<EmptyNode>();
            _backwardNode = Build<EmptyNode>();
            //_rotationNode = RotationNode.Build(Current);

            _forwardNode.OnDragEvent += OnDrag;
            _backwardNode.OnDragEvent += OnDrag;
            OnDragEvent += OnDrag;

            //_rotationNode.transform.parent = transform;
            //_rotationNode.transform.position = Vector3.zero;

        }
        public override void OnNotifySegmentChange()
        {
            base.OnNotifySegmentChange();
            if(Current != null)
                transform.position = Current.TrackSegment.transform.TransformPoint(Current.TrackSegment.curves.Last().p3);
        }
        protected override void Update()
        {
            base.Update();
           
        }
        private void OnDrag(BaseNode node)
        {
            Vector3 forwardNodePos = _forwardNode.transform.TransformPoint(Vector3.zero);
            Vector3 backNodePos = _backwardNode.transform.TransformPoint(Vector3.zero);
            Vector3 currentNodePos = transform.TransformPoint(Vector3.zero);

            if (node == _forwardNode)
            {
            }
            else if (node == _backwardNode)
            {
                
            }
            else if (node == this)
            {
                Debug.Log("updating pos of zero node");
                Forward.TrackSegment.curves.First().p0  = Forward.TrackSegment.transform.InverseTransformPoint(currentNodePos);
                Current.TrackSegment.curves.Last().p3 = Current.TrackSegment.transform.InverseTransformPoint(currentNodePos);
                
                
                Current.CalculateStartBinormal();
                Current.Invalidate = true;
                Forward.Invalidate = true;

            }
        }

        public override void OnHold()
        {
            base.OnHold();
        }

        public void onActivate(RaycastHit hit)
        {
            //_isActive = true;
        }

        public void onDeactivate()
        {
           // _isActive = false;
        }
      
    }
}