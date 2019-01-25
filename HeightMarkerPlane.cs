using System.Collections.Generic;
using UnityEngine;

namespace TrackEdit
{
    public class HeightMarkerPlane : MonoBehaviour
    {
        public TrackSegment4 TrackSegment { get; set; }
        private float _update = 0.0f;
        private void Update()
        {
            if (TrackSegment != null)
            {
                transform.parent = TrackSegment.transform;
                
                if (Time.time - _update > .5f)
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

                    var sample = TrackSegment.getLength() / Mathf.RoundToInt(TrackSegment.getLength() / .2f);
                    var pos = 0.0f;

                    var tForDistance = TrackSegment.getTForDistance(0);
                    var position = TrackSegment.getPoint(tForDistance);

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
                    while (pos < TrackSegment.getLength())
                    {
                        tForDistance = TrackSegment.getTForDistance(pos);
                        pos += sample;

                        position = TrackSegment.getPoint(tForDistance);

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
                    _update = Time.time;
                }
            }
        }

    }
}