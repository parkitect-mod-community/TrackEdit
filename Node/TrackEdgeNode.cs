using System.Linq;
using UnityEngine;

namespace TrackEdit.Node
{
    public class TrackEdgeNode: EmptyNode, IActivatable
    {

        public TrackSegmentHandler Forward { get; set; }
        public TrackSegmentHandler Current { get; set; }

       // private bool _isActive = false;

        private RotationNode _rotationNode;
        private EmptyNode _forwardNode;
        private EmptyNode _backwardNode;
        
        private LineRenderer _lineRenderer;

        protected override void Awake()
        {
            base.Awake();
            _forwardNode = Build<EmptyNode>();
            _backwardNode = Build<EmptyNode>();
            _rotationNode = RotationNode.Build(Current);

            _forwardNode.OnHoldEvent += OnHold;
            _backwardNode.OnHoldEvent += OnHold;
            OnHoldEvent += OnHold;
            
            _forwardNode.OnBeginHoldEvent += OnBeginHold;
            _backwardNode.OnBeginHoldEvent += OnBeginHold;
            OnBeginHoldEvent += OnBeginHold;

            _forwardNode.transform.parent = transform;
            _backwardNode.transform.parent = transform;
            _rotationNode.transform.parent = transform;

            
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.positionCount = 3;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.sharedMaterial = EmptyNode.DefaultNodeMaterial();
            _lineRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.Camera;
            _lineRenderer.allowOcclusionWhenDynamic = true;
            _lineRenderer.startWidth = .1f;
            _lineRenderer.enabled = false;
            onDeactivate();

        }
        public override void OnNotifySegmentChange()
        {
            base.OnNotifySegmentChange();
            if (Current != null)
            {
                _rotationNode.Handler = Current;
                transform.position =
                    Current.TrackSegment.transform.TransformPoint(Current.TrackSegment.curves.Last().p3);
            }
        }
        protected override void Update()
        {
            base.Update();
 
        }



        
        private void OnBeginHold(BaseNode node)
        {
            if (Current != null)
            {
                transform.position = Current.TrackSegment.transform.TransformPoint(Current.TrackSegment.curves.Last().p3);
                _backwardNode.transform.position = Current.TrackSegment.transform.TransformPoint(Current.TrackSegment.curves.Last().p2);
            }

            if (Forward != null)
            {
                _forwardNode.transform.position = Forward.TrackSegment.transform.TransformPoint(Forward.TrackSegment.curves.First().p1);
            }

        }


        private void OnDestroy()
        {
            if (_forwardNode != null)
                Destroy(_forwardNode.gameObject);
            if (_backwardNode != null)
                Destroy(_backwardNode.gameObject);
            if (_rotationNode != null)
                Destroy(_rotationNode.gameObject);
        }


        private void OnHold(BaseNode node)
        {
            
            Vector3 currentNodePos = transform.position;
            Vector3 forwardNodePos = _forwardNode.transform.position;
            Vector3 backNodePos = _backwardNode.transform.position;
            float forwardMagnitude = (forwardNodePos - currentNodePos).magnitude;
            float backwardMagnitude = (backNodePos - currentNodePos).magnitude;

            if (node == _forwardNode)
            {
                Vector3 dir = (forwardNodePos - currentNodePos) * -1;
                backNodePos = currentNodePos + dir.normalized * backwardMagnitude;
                _backwardNode.transform.position = backNodePos;

            }
            else if (node == _backwardNode)
            {
                Vector3 dir = (backNodePos - currentNodePos) * -1;
                forwardNodePos = currentNodePos + dir.normalized * forwardMagnitude;
                _forwardNode.transform.position = forwardNodePos;

            }

            if (Forward != null)
            {
                Forward.TrackSegment.curves.First().p0 =
                    Forward.TrackSegment.transform.InverseTransformPoint(currentNodePos);
                Forward.TrackSegment.curves.First().p1 =
                    Forward.TrackSegment.transform.InverseTransformPoint(forwardNodePos);

                Forward.RecalculateSegment();
                Forward.Invalidate = true;
            }

            if (Current != null)
            {
                Current.TrackSegment.curves.Last().p3 =
                    Current.TrackSegment.transform.InverseTransformPoint(currentNodePos);
                Current.TrackSegment.curves.Last().p2 =
                    Current.TrackSegment.transform.InverseTransformPoint(backNodePos);

                Current.RecalculateSegment();
                Current.Invalidate = true;

            }

            
            if (Forward != null)
            {
                _lineRenderer.positionCount = 3;
                _lineRenderer.SetPosition(2, _forwardNode.transform.position + NodeOffset);
            }
            else
            {
                _lineRenderer.positionCount = 2;
            }
            

            _lineRenderer.SetPosition(0, _backwardNode.transform.position + NodeOffset);               
            _lineRenderer.SetPosition(1, transform.position + NodeOffset);

        }

        public override void OnHold()
        {
            base.OnHold();
        }

        public void onActivate(RaycastHit hit)
        {
            if(Forward != null)
                _forwardNode.gameObject.SetActive(true);
            _backwardNode.gameObject.SetActive(true);
            _rotationNode.gameObject.SetActive(true);
            _lineRenderer.enabled = true;
        }

        public void onDeactivate()
        {
            _forwardNode.gameObject.SetActive(false);
            _backwardNode.gameObject.SetActive(false);
            _rotationNode.gameObject.SetActive(false);
            _lineRenderer.enabled = false;
        }
      
    }
}