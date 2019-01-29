using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TrackEdit.Node;
using UnityEngine;

namespace TrackEdit
{
    public class TrackSegmentHandler : MonoBehaviour
    {

        private bool _isSupportsInvalid;
        private float _meshGenerationTime;

        public bool Invalidate { get; set; }
        public TrackSegment4 TrackSegment { get; private set; }
        public TrackEditHandler Handler { get; set; }
        
        private TrackEdgeNode _edgeNode;

        public void Awake()
        {
            _edgeNode = TrackEdgeNode.Build<TrackEdgeNode>();
            TrackSegment = this.gameObject.GetComponent<TrackSegment4>();
        }
        
        

        public void NotifySegmentChange()
        {
            var nextSegment = GetNextSegment(true);
            var previousSegment = GetPreviousSegment(true);
            
            if(IsConnectedForwardSegment())
                _edgeNode.Forward = nextSegment;
            _edgeNode.Current = this;
            _edgeNode.OnNotifySegmentChange();

        }

        public int GetIndexOfSegment()
        {
            return TrackSegment.track.trackSegments.IndexOf(TrackSegment);
        }

        

        //calculate new rotation for segment and update the total and delta rotation
        public void CalculateWithNewTotalRotation(float newRotation)
        {
            var nextSegment = GetNextSegment(true);
            var previousSegment = GetPreviousSegment(true);

            var diff = newRotation - TrackSegment.totalRotation;
            if (previousSegment != null)
            {
                TrackSegment.deltaRotation += diff;
                TrackSegment.totalRotation += diff;
                TrackSegment.calculateLengthAndNormals(previousSegment.TrackSegment);
            }

            if (nextSegment != null) nextSegment.RecalculateSegment();
        }


        public TrackSegmentHandler GetNextSegment(bool hasToBeConnected)
        {
            if (TrackSegment.isConnectedToNextSegment || !hasToBeConnected)
            {
                var track = Handler.TrackRide.Track;
                return track.trackSegments[track.getNextSegmentIndex(track.trackSegments.IndexOf(TrackSegment))]
                    .GetComponent<TrackSegmentHandler>();
            }

            return null;
        }

        public TrackSegmentHandler GetPreviousSegment(bool hasToBeConnected)
        {
            if (TrackSegment.isConnectedToPreviousSegment || !hasToBeConnected)
            {
                var track = Handler.TrackRide.Track;
                return
                    track.trackSegments[track.getPreviousSegmentIndex(track.trackSegments.IndexOf(TrackSegment))]
                        .GetComponent<TrackSegmentHandler>();
            }

            return null;
        }

        public bool IsConnected(TrackSegmentHandler segment)
        {
            TrackSegmentHandler next = GetNextSegment(true);
            TrackSegmentHandler previous = GetPreviousSegment(true);
            if (segment == previous)
                return previous.TrackSegment.isConnectedTo(TrackSegment);
            if (segment == next)
                return TrackSegment.isConnectedTo(next.TrackSegment);
            return false;
        }

        public bool IsConnectedForwardSegment()
        {
            TrackSegmentHandler next = GetNextSegment(true);
            if (next == null)
                return false;
            return TrackSegment.isConnectedTo(next.TrackSegment);
        }
        public bool IsConnectedPreviousSegment()
        {
            TrackSegmentHandler previous = GetPreviousSegment(true);
            if (previous == null)
                return false;
            return previous.TrackSegment.isConnectedTo(TrackSegment);
        }

        public bool ConnectWithForwardSegment(TrackSegmentHandler next)
        {
            TrackSegment.isConnectedToNextSegment = true;
            next.TrackSegment.isConnectedToPreviousSegment = true;


            var nextFirstCurve = next.TrackSegment.curves.First();
            var currentLastCurve = TrackSegment.curves.Last();

            float magnitude = Mathf.Abs((
                next.TrackSegment.transform.TransformPoint(nextFirstCurve.p0) -
                next.TrackSegment.transform.TransformPoint(nextFirstCurve.p1)).magnitude);

            TrackSegment.curves.Last().p2 = TrackSegment.transform.InverseTransformPoint(
                TrackSegment.transform.TransformPoint(currentLastCurve.p3) +
                next.TrackSegment.getTangentPoint(0f) * -1f * magnitude);


            next.TrackSegment.calculateLengthAndNormals(TrackSegment);
            next.RecalculateSegment();
            Invalidate = true;
            return true;
        }


        private void RecalculateSegment()
        {
            typeof(TrackSegment4).GetMethod("clearLength", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(TrackSegment, new object[] { });
            
            var previousSegment = GetPreviousSegment(true);
            if (previousSegment != null)
            {
                var nextSegment = GetNextSegment(true);

                typeof(TrackSegment4).GetField("startBinormal",BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic).SetValue(TrackSegment,
                    TrackSegment.transform.InverseTransformDirection(Vector3.Cross(
                        previousSegment.TrackSegment.getNormal(1f), previousSegment.TrackSegment.getTangentPoint(1f))));

                if (nextSegment != null)
                {
                    nextSegment.TrackSegment.calculateLengthAndNormals(TrackSegment);

                    //try to match the curve 
                    for (var x = 0; x < 10; x++)
                    {
                        TrackSegment.deltaRotation -= MathHelper.AngleSigned(
                            Quaternion.AngleAxis(0, nextSegment.TrackSegment.getTangentPoint(0.0f)) *
                            nextSegment.TrackSegment.getNormalPoint(0.0f),
                            Quaternion.AngleAxis(TrackSegment.deltaRotation, TrackSegment.getTangentPoint(1.0f)) *
                            TrackSegment.getNormalPoint(1.0f), TrackSegment.getTangentPoint(1.0f));
                        TrackSegment.totalRotation =
                            previousSegment.TrackSegment.totalRotation +
                            TrackSegment.deltaRotation; // + TrackSegment.getAdditionalRotation ();
                        TrackSegment.calculateLengthAndNormals(previousSegment.TrackSegment);
                        if (previousSegment.TrackSegment.isConnectedTo(nextSegment.TrackSegment))
                            break;
                    }
                }
                else
                {
                    TrackSegment.totalRotation =
                        previousSegment.TrackSegment.totalRotation +
                        TrackSegment.deltaRotation; // + TrackSegment.getAdditionalRotation ();
                    TrackSegment.calculateLengthAndNormals(previousSegment.TrackSegment);
                }
            }
            TrackSegment.calculateLengthAndNormals(null);

        }

        
        
        private void ResetMeshForTrackSegment(TrackSegment4 segment)
        {
            typeof(TrackSegment4).GetMethod("onKill", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(segment,new object[]{});
            
            var generatedMesh = typeof(TrackSegment4).GetField("generatedMeshes",
                BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);
            if (generatedMesh != null)
            {
                var meshes = (List<Mesh>) generatedMesh.GetValue(segment);
                foreach (var m in meshes) Destroy(m);
                meshes.Clear();
            }

            foreach (Transform child in segment.gameObject.transform)
            {
                var meshFilter = child.gameObject.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Destroy(meshFilter.mesh);
                    Destroy(meshFilter.sharedMesh);
                }

                Destroy(child.gameObject);
            }

            MouseCollisions.Instance.removeColliders(segment, segment.gameObject);
            //UnityEngine.Object.DestroyImmediate( segment.gameObject.GetComponent<MouseCollider> ());
            Destroy(segment.gameObject.GetComponent<MeshCollider>());
            Destroy(segment.gameObject.GetComponent<BoundingMesh>());
        }



        private void OnDestroy()
        {
            if (_edgeNode != null)
                Destroy(_edgeNode.gameObject);

            TrackSegmentHandler nextHandler = GetNextSegment(false);
            if (nextHandler != null)
            {
                nextHandler.NotifySegmentChange();
            }
            
            TrackSegmentHandler previousHandler = GetPreviousSegment(false);
            if (previousHandler != null)
            {
                previousHandler.NotifySegmentChange();
            }


        }


        private void Update()
        { 
            if (Invalidate && Time.time - _meshGenerationTime > .05f )
            {
                if(GetNextSegment(TrackSegment) == null) Handler.TrackBuilder.generateNewGhost();

                ResetMeshForTrackSegment(TrackSegment);
                RecalculateSegment();

//                typeof(TrackSegment4).GetField("localCrossedTiles",BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic).SetValue(TrackSegment, null);
//                TrackSegment.generateMesh(TrackSegment.track.TrackedRide.meshGenerator);

                TrackSegment.Initialize();
      
                _meshGenerationTime = Time.time;
                Invalidate = false;
            }

        }
    }
}