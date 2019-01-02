using System.Reflection;
using UnityEngine;

namespace TrackEdit.StateMachine
{
    public class DraggableState : IState
    {
        private readonly SharedStateData _stateData;

        private float _gridSubdivision = 1.0f;
        private bool _isGridActive;
        private bool _verticalDragState;

        protected Vector3 DragPosition = Vector3.zero;

        public DraggableState(SharedStateData stateData)
        {
            _stateData = stateData;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var point = ray.GetPoint(stateData.Distance);

            if (Input.GetKey(Main.Configuration.Settings.VerticalKey))
            {
                stateData.Offset = new Vector3(stateData.Offset.x, stateData.Selected.transform.position.y - point.y,
                    stateData.Offset.z);
                _verticalDragState = true;
            }
        }

        public virtual void Update(FiniteStateMachine stateMachine)
        {
            typeof(CameraController).GetMethod("stopLocksAndPans", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(GameController.Instance.cameraController, new Object[] { });

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var point = ray.GetPoint(_stateData.Distance);
            Vector3 position;
            if (!_verticalDragState)
            {
                position = new Vector3(point.x, _stateData.FixedY, point.z) +
                           new Vector3(_stateData.Offset.x, _stateData.Offset.y, _stateData.Offset.z);
            }
            else
            {
                _stateData.FixedY = point.y;
                position = new Vector3(_stateData.Selected.position.x, _stateData.FixedY,
                               _stateData.Selected.position.z) + new Vector3(0, _stateData.Offset.y, 0);
            }

            if (Input.GetKeyDown(Main.Configuration.Settings.VerticalKey))
            {
                _stateData.Offset = new Vector3(_stateData.Offset.x, _stateData.Selected.transform.position.y - point.y,
                    _stateData.Offset.z);
                _verticalDragState = true;
            }
            else if (Input.GetKeyUp(Main.Configuration.Settings.VerticalKey))
            {
                _verticalDragState = false;
                _stateData.Offset = _stateData.Selected.transform.position - point;
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

            DragPosition = new Vector3(Mathf.Round(position.x * 10.0f) / 10.0f, Mathf.Round(position.y * 10.0f) / 10.0f,
                Mathf.Round(position.z * 10.0f) / 10.0f);
            if (_isGridActive)
                DragPosition = new Vector3(Mathf.Round(position.x * _gridSubdivision) / _gridSubdivision,
                    Mathf.Round(position.y * _gridSubdivision) / _gridSubdivision,
                    Mathf.Round(position.z * _gridSubdivision) / _gridSubdivision);
            TrackUiHandle.Instance.TrackBuilder.generateNewGhost();
        }

        public virtual void Unload()
        {
            resetToDefaultGrid();
        }

        private void resetToDefaultGrid()
        {
            GameController.Instance.terrainGridProjector.setHighIntensityEnabled(false);
            GameController.Instance.terrainGridBuilderProjector.setHighIntensityEnabled(false);
            GameController.Instance.terrainGridBuilderProjector.setGridSubdivision(1f);
        }
    }
}