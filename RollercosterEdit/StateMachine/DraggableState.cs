using System;
using RollercoasterEdit;
using UnityEngine;

namespace RollercoasterEdit
{
    public class DraggableState : IState
    {
        private SharedStateData _stateData;

        protected Vector3 dragPosition = Vector3.zero;
        protected bool verticalDragState = false;
        protected bool isGridActive = false;

        private float gridSubdivision = 1.0f;

        public DraggableState (SharedStateData stateData)
        {
            this._stateData = stateData;

            var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            Vector3 point = ray.GetPoint (_stateData.Distance);
            Vector3 position = Vector3.zero;

            if (Input.GetKey (Main.Configeration.VerticalKey)) {
                _stateData.Offset = new Vector3(_stateData.Offset.x,_stateData.Selected.transform.position.y - point.y, _stateData.Offset.z);
                verticalDragState = true;

            }
        }

        public virtual void Update (FiniteStateMachine stateMachine)
        {
            var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            Vector3 point = ray.GetPoint (_stateData.Distance);
            Vector3 position = Vector3.zero;
            if (!verticalDragState) {
                position = new Vector3 (point.x, _stateData.FixedY, point.z) + new Vector3(_stateData.Offset.x, _stateData.Offset.y, _stateData.Offset.z);

            } else
            {
                _stateData.FixedY = point.y;
                position = new Vector3 (_stateData.Selected.position.x, _stateData.FixedY, _stateData.Selected.position.z) + new Vector3(0, _stateData.Offset.y, 0);
            }

            if (Input.GetKeyDown (Main.Configeration.VerticalKey)) {
                _stateData.Offset = new Vector3(_stateData.Offset.x,_stateData.Selected.transform.position.y - point.y, _stateData.Offset.z);
                verticalDragState = true;

            } else if (Input.GetKeyUp (Main.Configeration.VerticalKey)) {
                verticalDragState = false;
                _stateData.Offset = (_stateData.Selected.transform.position - point);

            }

                if (InputManager.getKey("BuildingSnapToGrid"))
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0))
                    {
                        this.gridSubdivision = 1;
                        GameController.Instance.terrainGridBuilderProjector.setGridSubdivision(this.gridSubdivision);
                    }
                    for (int i = 1; i <= 9; i++)
                    {
                        if (Input.GetKeyDown(i + string.Empty))
                        {
                            this.gridSubdivision = (float)i;
                            GameController.Instance.terrainGridBuilderProjector.setGridSubdivision(this.gridSubdivision);
                        }
                    }
                }

            if (InputManager.getKeyDown ("BuildingSnapToGrid")) {
                isGridActive = true;
                GameController.Instance.terrainGridProjector.setHighIntensityEnabled (true);
                GameController.Instance.terrainGridBuilderProjector.setHighIntensityEnabled (true);
                GameController.Instance.terrainGridBuilderProjector.setGridSubdivision (this.gridSubdivision);
              
            } else if (InputManager.getKeyUp ("BuildingSnapToGrid")) {
                this.resetToDefaultGrid ();
                isGridActive = false;
            } 

            dragPosition = new Vector3 (Mathf.Round (position.x * 10.0f) /  10.0f, Mathf.Round (position.y *  10.0f) / 10.0f, Mathf.Round (position.z * 10.0f) /  10.0f);
            if (isGridActive) {
                dragPosition = new Vector3 (Mathf.Round (position.x * gridSubdivision) / gridSubdivision, Mathf.Round (position.y * gridSubdivision) / gridSubdivision, Mathf.Round (position.z * gridSubdivision) / gridSubdivision);

            }
                

           
                

        }

        private void resetToDefaultGrid()
        {
            GameController.Instance.terrainGridProjector.setHighIntensityEnabled(false);
            GameController.Instance.terrainGridBuilderProjector.setHighIntensityEnabled(false);
            GameController.Instance.terrainGridBuilderProjector.setGridSubdivision(1f);
        }

        public virtual void Unload ()
        {
            resetToDefaultGrid ();
        }
    }
}

