using System;
using UnityEngine;

namespace RollercoasterEdit
{
    public class FreeDragState : IState
    {
		private BuilderHeightMarker _heightMaker;
        private bool _verticalDragState;
        private SharedStateData _stateData;
        public FreeDragState (SharedStateData stateData)
        {
			
			this._stateData = stateData;
			_heightMaker = UnityEngine.Object.Instantiate<BuilderHeightMarker>(ScriptableSingleton<AssetManager>.Instance.builderHeightMarkerGO);
			_heightMaker.attachedTo = stateData.Selected.transform;
			_heightMaker.heightChangeDelta = .01f;


        }

        public void Update(FiniteStateMachine stateMachine)
        {
            var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
            Vector3 point = ray.GetPoint (_stateData.Distance);
            Vector3 position = Vector3.zero;
            if (!_verticalDragState) {
                position = new Vector3 (point.x, _stateData.FixedY, point.z) + new Vector3(_stateData.Offset.x, _stateData.Offset.y, _stateData.Offset.z);

            } else
            {
                _stateData.FixedY = point.y;
                position = new Vector3 (_stateData.Selected.position.x, _stateData.FixedY, _stateData.Selected.position.z) + new Vector3(0, _stateData.Offset.y, 0);
            }

            TrackNode trackNode = _stateData.Selected.gameObject.GetComponent<TrackNode> ();

            if (Input.GetKeyDown (Main.Configeration.VerticalKey)) {
                _stateData.Offset = new Vector3(_stateData.Offset.x,_stateData.Selected.transform.position.y - point.y, _stateData.Offset.z);
                _verticalDragState = true;
 
            } else if (Input.GetKeyUp (Main.Configeration.VerticalKey)) {
                _verticalDragState = false;
                _stateData.Offset = (_stateData.Selected.transform.position - point);

            }

            //calculate the match for the node segment
            TrackNodeHelper.CalculateMatch (trackNode, position);

            
			var nextSegment = trackNode.TrackSegmentModify.GetNextSegment (false);
			if (!_stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.TrackSegment.isConnectedToNextSegment) {
			
			
				if (_stateData.Selected.gameObject.GetComponent<TrackNode> ().NodePoint == TrackNode.NodeType.P3 && (position - nextSegment.GetFirstCurve.P0.GetGlobal ()).sqrMagnitude < .2f) {
					
					float magnitude = Mathf.Abs ((nextSegment.GetFirstCurve.P0.GetGlobal () - nextSegment.GetFirstCurve.P1.GetGlobal ()).magnitude);


					_stateData.Selected.gameObject.GetComponent<TrackNode> ().SetPoint (nextSegment.GetFirstCurve.P0.GetGlobal ());
					_stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.GetLastCurve.P2.SetPoint (_stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.GetLastCurve.P3.GetGlobal () + (nextSegment.TrackSegment.getTangentPoint (0f) * -1f * magnitude));
					_stateData.Selected.gameObject.GetComponent<TrackNode> ().CalculateLenghtAndNormals ();
					_stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.GetFirstCurve.P0.TrackSegmentModify.CalculateStartBinormal (false);
					_stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.GetLastCurve.ClearExtrudeNode ();


                    nextSegment.GetFirstCurve.P0.CalculateLenghtAndNormals ();
					if (Input.GetMouseButtonUp (0)) {
                        _stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.ConnectWithForwardSegment (nextSegment);
                        stateMachine.ChangeState (new IdleState (_stateData));
					}
					nextSegment.Invalidate = true;

					_stateData.Selected.gameObject.GetComponent<TrackNode> ().TrackSegmentModify.Invalidate = true;

				}
			}

            if (Input.GetMouseButtonUp (0)) {
				stateMachine.ChangeState(new IdleState (_stateData));
            }
        }

		public void Unload()
		{
			UnityEngine.Object.Destroy (_heightMaker.gameObject);
		}
    }
}

