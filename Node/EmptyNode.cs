/**
* Copyright 2019 Michael Pollind
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using UnityEngine;

namespace TrackEdit.Node
{
    public class EmptyNode : BaseNode
    {


        private float _fixedY = 0;
        private Vector3 _offset = Vector3.zero;
        private float _distance = 0;

        private float _gridSubdivision = 1.0f;

        private bool _verticalDragState;
        private bool _isGridActive;

        private Vector3 _dragPosition = Vector3.zero;

        public void OnRemove()
        {
        }

        protected override void Awake()
        {

        }

        public override void OnNotifySegmentChange()
        {

        }

        protected override void Update()
        {
            if (InputManager.getKeyUp("BuildingSnapToGrid"))
            {
                resetToDefaultGrid();
                _isGridActive = false;
            }

            if (Input.GetKeyUp(Main.Configuration.Settings.VerticalKey))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var point = ray.GetPoint(_distance);

                _verticalDragState = false;
                _offset = this.transform.position - point;
            }
        }

        public override void OnBeginHold(RaycastHit hit)
        {
            base.OnBeginHold(hit);

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            _distance = (ray.origin - hit.point).magnitude;
            var point = ray.GetPoint(_distance);

            _fixedY = hit.transform.position.y;
            _offset = new Vector3(hit.transform.position.x - hit.point.x, 0, hit.transform.position.z - hit.point.z);

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
            if (!_verticalDragState)
            {
                transform.position = new Vector3(point.x, _fixedY, point.z) +
                                     new Vector3(_offset.x, _offset.y, _offset.z);
            }
            else
            {
                _fixedY = point.y;
                transform.position = new Vector3(transform.position.x, _fixedY,
                                         transform.position.z) + new Vector3(0, _offset.y, 0);
            }

            if (Input.GetKeyDown(Main.Configuration.Settings.VerticalKey))
            {
                _offset = new Vector3(_offset.x, transform.position.y - point.y, _offset.z);
                _verticalDragState = true;
            }

            if (InputManager.getKey("BuildingSnapToGrid"))
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    _gridSubdivision = 1;
                    GameController.Instance.terrainGridBuilderProjector.setGridSubdivision(_gridSubdivision);
                }

                for (var i = 1; i <= 9; i++)
                {
                    if (Input.GetKeyDown(i + string.Empty))
                    {
                        _gridSubdivision = i;
                        GameController.Instance.terrainGridBuilderProjector.setGridSubdivision(_gridSubdivision);
                    }
                }
            }

            if (InputManager.getKeyDown("BuildingSnapToGrid"))
            {
                _isGridActive = true;
                GameController.Instance.terrainGridProjector.setHighIntensityEnabled(true);
                GameController.Instance.terrainGridBuilderProjector.setHighIntensityEnabled(true);
                GameController.Instance.terrainGridBuilderProjector.setGridSubdivision(_gridSubdivision);
            }

            transform.position = new Vector3(Mathf.Round(transform.position.x * 10.0f) / 10.0f,
                Mathf.Round(transform.position.y * 10.0f) / 10.0f,
                Mathf.Round(transform.position.z * 10.0f) / 10.0f);
            if (_isGridActive)
                transform.position = new Vector3(
                    Mathf.Round(transform.position.x * _gridSubdivision) / _gridSubdivision,
                    Mathf.Round(transform.position.y * _gridSubdivision) / _gridSubdivision,
                    Mathf.Round(transform.position.z * _gridSubdivision) / _gridSubdivision);

            base.OnHold();

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


        public static readonly Vector3 NodeOffset = new Vector3(0, .6f, 0);
        public static readonly float NodeRadius = .1f;

        private static Mesh _nodeCircleMesh = null;
        private static Material _nodeMaterial = null;
        private static Material _nodeErrorMaterial = null;


        private static readonly int _TintColor = Shader.PropertyToID("_Color");

        public static T Build<T>() where T : EmptyNode
        {
            if (_nodeCircleMesh == null)
            {
                _nodeCircleMesh = GameObjectUtility.CreateCircle(NodeRadius, 10);
            }

            GameObject result = new GameObject();
            // result.layer = LayerMasks.ID_COASTER_TRACKS;

            SphereCollider sphereCollider = result.AddComponent<SphereCollider>();
            sphereCollider.center = NodeOffset;
            sphereCollider.radius = .1f;

            GameObject button = new GameObject("button");

            MeshFilter meshFilter = button.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = _nodeCircleMesh;
            MeshRenderer meshRenderer = button.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = DefaultNodeMaterial();
            button.transform.parent = result.transform;
            button.transform.localPosition = NodeOffset;
            button.AddComponent<FaceCamera>();

            return result.AddComponent<T>();
        }

        public static Material DefaultNodeMaterialError()
        {
            if (_nodeErrorMaterial  == null)
            {
                _nodeErrorMaterial  = new Material(Shader.Find("GUI/Text Shader"));
                _nodeErrorMaterial.color = new Color(1f, 0.00f, 0f, 1f);

                // _nodeErrorMaterial.SetColor("_TintColor", new Color(255, 0, 0, 100));
            }
            return _nodeErrorMaterial ;
        }


        public static Material DefaultNodeMaterial()
        {
            if (_nodeMaterial == null)
            {
                _nodeMaterial = new Material(Shader.Find("GUI/Text Shader"));
                _nodeMaterial.color =new Color(1f, 1f, 1f, 1f);
                // _nodeMaterial.SetColor("_TintColor", new Color(255, 255, 255, 100));
            }
            return _nodeMaterial;
        }



    }
}
