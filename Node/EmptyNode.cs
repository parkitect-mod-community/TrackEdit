using System;
using UnityEngine;

namespace TrackEdit.Node
{
    public class EmptyNode : BaseNode
    {
      
        public event OnPositionChange OnDragEvent;
        
        private Transform _face;

        private float _fixedY = 0;
        private Vector3 _offset = Vector3.zero;
        private float _distance = 0;
        
        private float _gridSubdivision = 1.0f;
        
        
        private bool _verticalDragState;
        private bool _isGridActive;
        
        private Vector3 _dragPosition = Vector3.zero;

        public void onRemove()
        {
        }

        public virtual void Awake()
        {
            _face = transform.Find("node");
        }

        public virtual void Update()
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                _face.LookAt(Camera.main.transform, Vector3.up);
            }

        }

        public override void OnPressed(RaycastHit hit)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _distance = (ray.origin - hit.point).magnitude;
            var point = ray.GetPoint(_distance);
            if (Input.GetKey(Main.Configuration.Settings.VerticalKey))
            {
               _offset = new Vector3(_offset.x, transform.position.y - point.y,
                    _offset.z);
                _verticalDragState = true;
            }
        }

        public override void OnHold()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var point = ray.GetPoint(_distance);
            Vector3 position;
            if (!_verticalDragState)
            {
                position = new Vector3(point.x, _fixedY, point.z) +
                           new Vector3(_offset.x, _offset.y, _offset.z);
            }
            else
            {
                _fixedY = point.y;
                position = new Vector3(transform.position.x, _fixedY,
                               transform.position.z) + new Vector3(0, _offset.y, 0);
            }
            
            if (Input.GetKeyDown(Main.Configuration.Settings.VerticalKey))
            {
                _offset = new Vector3(_offset.x, transform.position.y - point.y,_offset.z);
                _verticalDragState = true;
            }
            else if (Input.GetKeyUp(Main.Configuration.Settings.VerticalKey))
            {
                _verticalDragState = false;
                _offset = this.transform.position - point;
            }

            if (InputManager.getKey("BuildingSnapToGrid"))
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    _gridSubdivision = 1;
                    GameController.Instance.terrainGridBuilderProjector.setGridSubdivision(_gridSubdivision);
                }

                for (var i = 1; i <= 9; i++)
                    if (Input.GetKeyDown(i + string.Empty))
                    {
                        _gridSubdivision = i;
                        GameController.Instance.terrainGridBuilderProjector.setGridSubdivision(_gridSubdivision);
                    }
            }

            if (InputManager.getKeyDown("BuildingSnapToGrid"))
            {
                _isGridActive = true;
                GameController.Instance.terrainGridProjector.setHighIntensityEnabled(true);
                GameController.Instance.terrainGridBuilderProjector.setHighIntensityEnabled(true);
                GameController.Instance.terrainGridBuilderProjector.setGridSubdivision(_gridSubdivision);
            }
            else if (InputManager.getKeyUp("BuildingSnapToGrid"))
            {
                resetToDefaultGrid();
                _isGridActive = false;
            }

            _dragPosition = new Vector3(Mathf.Round(position.x * 10.0f) / 10.0f, Mathf.Round(position.y * 10.0f) / 10.0f,
                Mathf.Round(position.z * 10.0f) / 10.0f);
            if (_isGridActive)
                _dragPosition = new Vector3(Mathf.Round(position.x * _gridSubdivision) / _gridSubdivision,
                    Mathf.Round(position.y * _gridSubdivision) / _gridSubdivision,
                    Mathf.Round(position.z * _gridSubdivision) / _gridSubdivision);
            
            if (OnDragEvent != null) OnDragEvent.Invoke(this);
        }
        
        private void resetToDefaultGrid()
        {
            GameController.Instance.terrainGridProjector.setHighIntensityEnabled(false);
            GameController.Instance.terrainGridBuilderProjector.setHighIntensityEnabled(false);
            GameController.Instance.terrainGridBuilderProjector.setGridSubdivision(1f);
        }

        public override void OnRelease()
        {
            resetToDefaultGrid();
        }
        
        private static Mesh _nodeCircleMesh = null;
        private static Material _nodeMaterial = null;
        private static readonly int _TintColor = Shader.PropertyToID("_TintColor");

        public static T Build<T> () where T : EmptyNode 
        {
            if(_nodeCircleMesh == null)
                _nodeCircleMesh = GameObjectUtility.CreateCircle(0.1f,10);;
            if (_nodeMaterial == null)
            {
                _nodeMaterial =  new Material(Shader.Find("UI/Default"));
                _nodeMaterial.SetColor(_TintColor, new Color(255, 255, 255, 100));
            }
           
            GameObject result = new GameObject();
            result.layer = LayerMasks.ID_COASTER_TRACKS;
            
            SphereCollider sphereCollider =  result.AddComponent<SphereCollider>();
            sphereCollider.center = new Vector3(0, 0.5f, 0.0f);
            sphereCollider.radius = 5;
                
            GameObject child = new GameObject();

            MeshFilter meshFilter = child.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = _nodeCircleMesh;
            MeshRenderer meshRenderer = child.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = _nodeMaterial;

            LineRenderer lineRenderer = child.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 3;
            lineRenderer.useWorldSpace = true;
            lineRenderer.sharedMaterial = _nodeMaterial;
            lineRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.Camera;
            lineRenderer.allowOcclusionWhenDynamic = true;
            lineRenderer.startWidth = .1f;
            
            child.transform.position = new Vector3(0,.5f,0);
            child.transform.parent = result.transform;
            child.name = "node";

            return result.AddComponent<T>();
        }

    }
}