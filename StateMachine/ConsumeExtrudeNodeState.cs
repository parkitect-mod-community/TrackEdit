using UnityEngine;

namespace TrackEdit.StateMachine
{
    public class ConsumeExtrudeNodeState : IState
    {
        private readonly SharedStateData _stateData;

        public ConsumeExtrudeNodeState(SharedStateData stateData)
        {
            _stateData = stateData;
        }

        public void Update(FiniteStateMachine stateMachine)
        {
            //the old distance between the p2-p3 node
            var curve = _stateData.Selected.GetComponent<ExtrudeNode>().TrackSegmentModify.GetLastCurve;
            var direction = curve.P2.GetGlobal() - curve.P3.GetGlobal();

            var trackSegment =
                Object.Instantiate(
                    ScriptableSingleton<AssetManager>.Instance.getPrefab<TrackSegment4>(Prefabs.Straight));

            trackSegment.isLifthill = TrackEditHandler.Instance.TrackBuilder.liftToggle.isOn;
            /* if (TrackUIHandle.instance.trackEditUI.chainToggle.isOn) {
                 trackSegment.isLifthill = true;
             }*/

            TrackEditHandler.Instance.TrackRide.Track.addSegment(trackSegment,
                _stateData.Selected.GetComponent<ExtrudeNode>().TrackSegmentModify.GetIndexOfSegment() + 1);
            trackSegment.Initialize();


            var modify = trackSegment.gameObject.AddComponent<TrackSegmentModify>();
            _stateData.Selected.GetComponent<ExtrudeNode>().TrackSegmentModify.ConnectWithForwardSegment(modify);


            //_stateData.Selected.GetComponent<ExtrudeNode>().TrackSegmentModify.GetLastCurve.P3

            modify.CalculateStartBinormal(true);

            _stateData.Selected = modify.GetLastCurve.P3.gameObject.transform;

            //re-apply p2-p3 because that is lost when the segments are merged
            TrackNodeHelper.CalculateMatch(curve.P2, curve.P3.GetGlobal() + direction);

            stateMachine.ChangeState(new FreeDragWithSmoothing(_stateData));

            //stateMachine.ChangeState (new IdleState (_stateData));
        }


        public void Unload()
        {
        }
    }
}