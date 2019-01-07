using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TrackEdit
{
    public class TrackSegmentModify : MonoBehaviour
    {
        private FieldInfo _biNormalField;

        private bool _isSupportsInvalid;
        private float _meshGenerationTime;
        private float _supportRegnerateTime;


        public bool Invalidate;
        public TrackSegment4 TrackSegment { get; private set; }
        public List<TrackNodeCurve> GetTrackCurves { get; } = new List<TrackNodeCurve>();
        public TrackNodeCurve GetLastCurve => GetTrackCurves[GetTrackCurves.Count - 1];
        public TrackNodeCurve GetFirstCurve => GetTrackCurves[0];

        private void Awake()
        {
            TrackSegment = GetComponent<TrackSegment4>();

            var flags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic;
            _biNormalField = typeof(TrackSegment4).GetField("startBinormal", flags);


            for (var x = 0; x < TrackSegment.curves.Count; x++)
            {
                var grouping = TrackNodeCurve.Grouping.Middle;

                if (x == 0)
                    grouping = TrackNodeCurve.Grouping.Start;
                if (x == TrackSegment.curves.Count - 1)
                    grouping = TrackNodeCurve.Grouping.End;
                if (TrackSegment.curves.Count == 1)
                    grouping = TrackNodeCurve.Grouping.Both;

                GetTrackCurves.Add(new TrackNodeCurve(TrackSegment.curves[x], this, grouping));
            }
        }

        public TrackNodeCurve GetNextCurve(TrackNodeCurve current)
        {
            var index = GetTrackCurves.IndexOf(current);
            if (index + 1 >= GetTrackCurves.Count)
            {
                var nextSegment = current.SegmentModify.GetNextSegment(true);
                if (nextSegment != null)
                    return nextSegment.GetFirstCurve;
                return null;
            }

            return GetTrackCurves[index + 1];
        }

        public int GetIndexOfSegment()
        {
            return TrackSegment.track.trackSegments.IndexOf(TrackSegment);
        }

        public TrackNodeCurve GetPreviousCurve(TrackNodeCurve current)
        {
            var index = GetTrackCurves.IndexOf(current);
            if (index - 1 < 0)
            {
                var previousCurve = current.SegmentModify.GetPreviousSegment(true);
                if (previousCurve != null)
                    return previousCurve.GetLastCurve;
            }

            return GetTrackCurves[index - 1];
        }

        public void CalculateStartBinormal(bool hasToBeconnected)
        {
            var previousSegment = GetPreviousSegment(true);
            if (previousSegment != null)
            {
                var nextSegment = GetNextSegment(true);

                _biNormalField.SetValue(TrackSegment,
                    TrackSegment.transform.InverseTransformDirection(Vector3.Cross(
                        previousSegment.TrackSegment.getNormal(1f), previousSegment.TrackSegment.getTangentPoint(1f))));

                if (nextSegment != null)
                {
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

            if (nextSegment != null) nextSegment.CalculateStartBinormal(true);
        }


        public TrackSegmentModify GetNextSegment(bool hasToBeconnected)
        {
            if (TrackSegment.isConnectedToNextSegment || !hasToBeconnected)
            {
                var track = TrackEditHandler.Instance.TrackRide.Track;
                return track.trackSegments[track.getNextSegmentIndex(track.trackSegments.IndexOf(TrackSegment))]
                    .GetComponent<TrackSegmentModify>();
            }

            return null;
        }

        public TrackSegmentModify GetPreviousSegment(bool hasToBeconnected)
        {
            if (TrackSegment.isConnectedToPreviousSegment || !hasToBeconnected)
            {
                var track = TrackEditHandler.Instance.TrackRide.Track;
                return track.trackSegments[track.getPreviousSegmentIndex(track.trackSegments.IndexOf(TrackSegment))]
                    .GetComponent<TrackSegmentModify>();
            }

            return null;
        }

        public bool ConnectWithForwardSegment(TrackSegmentModify next)
        {
            TrackSegment.isConnectedToNextSegment = true;
            next.TrackSegment.isConnectedToPreviousSegment = true;


            var magnitude =
                Mathf.Abs((next.GetFirstCurve.P0.GetGlobal() - next.GetFirstCurve.P1.GetGlobal()).magnitude);

            GetLastCurve.P2.SetPoint(GetLastCurve.P3.GetGlobal() +
                                     next.TrackSegment.getTangentPoint(0f) * -1f * magnitude);


            next.TrackSegment.calculateLengthAndNormals(GetLastCurve.SegmentModify.TrackSegment);
            next.CalculateStartBinormal(false);
            Invalidate = true;
            return true;
        }


        private void OnDestroy()
        {
            for (var x = 0; x < GetTrackCurves.Count; x++) GetTrackCurves[x].Destroy();
        }

        private void ResetMeshForTrackSegment(TrackSegment4 segment)
        {
            var generatedMesh = typeof(TrackSegment4).GetField("generatedMeshes",
                BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);
            if (generatedMesh != null)
            {
                var meshes = (List<Mesh>) generatedMesh.GetValue(segment);
                foreach (var m in meshes) DestroyImmediate(m);
                meshes.Clear();
            }

            foreach (Transform child in segment.gameObject.transform)
            {
                var meshFilter = child.gameObject.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    DestroyImmediate(meshFilter.mesh);
                    DestroyImmediate(meshFilter.sharedMesh);
                }

                DestroyImmediate(child.gameObject);
            }
            MouseCollisions.Instance.removeColliders(segment,segment.gameObject);
            //UnityEngine.Object.DestroyImmediate( segment.gameObject.GetComponent<MouseCollider> ());
            DestroyImmediate(segment.gameObject.GetComponent<MeshCollider>());
            DestroyImmediate(segment.gameObject.GetComponent<BoundingMesh>());
        }

        private void GenerateHeightMarkerTrack(TrackSegment4 trackSegment)
        {
            var heightTransform = transform.Find("HeightMarkerPlane");
            GameObject heightMarkerGo;
            MeshFilter meshFilter;
            if (heightTransform == null)
            {
                heightMarkerGo = new GameObject("HeightMarkerPlane");
                meshFilter = heightMarkerGo.AddComponent<MeshFilter>();
                var meshRenderer = heightMarkerGo.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = GameObjectUtility.GetMaterialPlane();
                heightMarkerGo.transform.SetParent(transform);
            }
            else
            {
                heightMarkerGo = heightTransform.gameObject;
                meshFilter = heightTransform.gameObject.GetComponent<MeshFilter>();
                meshFilter.mesh.Clear();
            }


            heightMarkerGo.transform.localPosition = Vector3.zero;
            heightMarkerGo.transform.localRotation = Quaternion.identity;

            var verticies = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();

            var sample = trackSegment.getLength() / Mathf.RoundToInt(trackSegment.getLength() / .2f);
            var pos = 0.0f;

            var tForDistance = trackSegment.getTForDistance(0);
            var position = trackSegment.getPoint(tForDistance);

            var terrain = GameController.Instance.park.getTerrain(transform.position);
            var vector = position;
            if (terrain != null) vector = terrain.getPoint(transform.position);
            var magnitude = (position - vector).magnitude;


            verticies.Add(transform.InverseTransformPoint(position));
            verticies.Add(
                transform.InverseTransformPoint(position + Vector3.down * magnitude *
                                                Mathf.Sign(position.y - vector.y)));

            uvs.Add(new Vector2(0, magnitude));
            uvs.Add(new Vector2(0, 0));

            var previous = position;
            float xoffset = 0;
            while (pos < trackSegment.getLength())
            {
                tForDistance = trackSegment.getTForDistance(pos);
                pos += sample;

                position = trackSegment.getPoint(tForDistance);

                terrain = GameController.Instance.park.getTerrain(position);
                vector = position;
                if (terrain != null) vector = terrain.getPoint(position);
                magnitude = (position - vector).magnitude;


                verticies.Add(transform.InverseTransformPoint(position));
                verticies.Add(
                    transform.InverseTransformPoint(
                        position + Vector3.down * magnitude * Mathf.Sign(position.y - vector.y)));

                xoffset += Vector3.Distance(previous, position);
                uvs.Add(new Vector2(xoffset, vector.y + magnitude));
                uvs.Add(new Vector2(xoffset, vector.y - 0));


                var last = verticies.Count - 1;
                triangles.Add(last - 3);
                triangles.Add(last - 2);
                triangles.Add(last - 1);

                triangles.Add(last - 1);
                triangles.Add(last - 2);
                triangles.Add(last);

                previous = position;
            }

            meshFilter.mesh.vertices = verticies.ToArray();
            meshFilter.mesh.triangles = triangles.ToArray();
            meshFilter.mesh.uv = uvs.ToArray();
        }

        private void Update()
        {
            if (Invalidate)
                _supportRegnerateTime = Time.time;

            if (Invalidate && Time.time - _meshGenerationTime > .05f)
            {
                _isSupportsInvalid = true;

                ResetMeshForTrackSegment(TrackSegment);
                TrackSegment.generateMesh(TrackEditHandler.Instance.TrackRide.meshGenerator);
                GenerateHeightMarkerTrack(TrackSegment);

                _meshGenerationTime = Time.time;
                Invalidate = false;
            }

            if (_isSupportsInvalid && Time.time - _supportRegnerateTime > .1f)
            {
                for (var x = 0; x < 5; x++)
                {
                    ResetMeshForTrackSegment(TrackSegment);
                    var localCrossTile = typeof(TrackSegment4).GetField("localCrossedTiles",
                        BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);
                    if (localCrossTile != null) localCrossTile.SetValue(TrackSegment, null);

                    TrackSegment.generateMesh(TrackEditHandler.Instance.TrackRide.meshGenerator);
                }

                _isSupportsInvalid = false;
            }

            foreach (var curves in GetTrackCurves) curves.Update();
        }
    }
}