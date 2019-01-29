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

        private bool _isSnapping = false;
        
        protected override void Awake()
        {
            base.Awake();
            _forwardNode = Build<EmptyNode>();
            _backwardNode = Build<EmptyNode>();
            _rotationNode = RotationNode.Build(Current);

            _forwardNode.OnHoldEvent += OnHoldHandler;
            _backwardNode.OnHoldEvent += OnHoldHandler;
            OnHoldEvent += OnHoldHandler;
            
            _forwardNode.OnBeginHoldEvent += OnBeginHoldHandler;
            _backwardNode.OnBeginHoldEvent += OnBeginHoldHandler;
            OnBeginHoldEvent += OnBeginHoldHandler;

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
            base.OnNotifySegmentChange();
        }
        protected override void Update()
        {
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

            base.Update();
 
        }

        public override void OnRelease()
        {
            base.OnRelease();
            if (_isSnapping && Current != null)
            {
                TrackSegmentHandler nextSegment = Current.GetNextSegment(false);
                if (nextSegment != null)
                { 
                    _forwardNode.gameObject.SetActive(true);
                    Current.ConnectWithForwardSegment(nextSegment);
                }
            }
        }

        private void OnBeginHoldHandler(BaseNode node)
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


        private float _oldRotation = 0.0f;
        private Vector3 _oldBackLocalPosition = Vector3.zero;

        private void TrySnap()
        {

            TrackSegmentHandler nextSegment = Current.GetNextSegment(false);
            if (Current != null && nextSegment != null && !Current.IsConnectedForwardSegment())
            {
//                Vector3 currentPos = nextSegment.TrackSegment.transform.TransformPoint(nextSegment.TrackSegment.curves.First().p0);
                Vector3 nextSegmentP0 =
                    nextSegment.TrackSegment.transform.TransformPoint(nextSegment.TrackSegment.curves.First().p0);
                Vector3 nextSegmentP1 =
                    nextSegment.TrackSegment.transform.TransformPoint(nextSegment.TrackSegment.curves.First().p1);

                Vector3 dir = nextSegmentP0 - nextSegmentP1;

                float dist = (this.transform.position - nextSegmentP0).magnitude;

                if (dist < .5f)
                {
                    transform.position = nextSegmentP0;
                    _backwardNode.transform.position =
                        nextSegmentP0 + dir.normalized *
                        (_backwardNode.transform.position - transform.position).magnitude;
                    _forwardNode.transform.position = nextSegmentP1;

                    Current.CalculateWithNewTotalRotation(nextSegment.GetStartRotation());
                    _isSnapping = true;

                }
                else
                {
                    if (_isSnapping)
                    {
                        _backwardNode.transform.localPosition = _oldBackLocalPosition;
                        Current.CalculateWithNewTotalRotation(_oldRotation);
                    }

                    _oldRotation = Current.GetCurrentTotalRotation();
                    _oldBackLocalPosition = _backwardNode.transform.localPosition;

                    _isSnapping = false;
                }
            }
        }

        private void OnHoldHandler(BaseNode node)
        {
            TrySnap();
            
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

                Forward.Invalidate = true;


                MeshRenderer mesh = transform.Find("button").GetComponent<MeshRenderer>();
                mesh.sharedMaterial = Current.IsConnected(Forward) ? DefaultNodeMaterial() : DefaultNodeMaterialError();
            }

            if (Current != null)
            {
                Current.TrackSegment.curves.Last().p3 =
                    Current.TrackSegment.transform.InverseTransformPoint(currentNodePos);
                Current.TrackSegment.curves.Last().p2 =
                    Current.TrackSegment.transform.InverseTransformPoint(backNodePos);

                Current.Invalidate = true;

            }


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