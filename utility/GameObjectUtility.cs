using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace TrackEdit
{
    public class GameObjectUtility
    {
        private GameObjectUtility()
        {
            
        }
        
        public static Mesh CreateCircle(float radius,int numOfPoints)
        {
            float angleStep = 360.0f / numOfPoints;
            List<Vector3> vertexList = new List<Vector3>();
            List<int> triangleList = new List<int>();
            Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, angleStep);
            // Make first triangle.
            vertexList.Add(new Vector3(0.0f, 0.0f, 0.0f));  // 1. Circle center.
            vertexList.Add(new Vector3(0.0f, radius, 0.0f));  // 2. First vertex on circle outline (radius = 0.5f)
            vertexList.Add(quaternion * vertexList[1]);     // 3. First vertex on circle outline rotated by angle)
            // Add triangle indices.
            triangleList.Add(0);
            triangleList.Add(1);
            triangleList.Add(2);
            for (int i = 0; i < numOfPoints - 1; i++)
            {
                triangleList.Add(0);                      // Index of circle center.
                triangleList.Add(vertexList.Count - 1);
                triangleList.Add(vertexList.Count);
                vertexList.Add(quaternion * vertexList[vertexList.Count - 1]);
            }
            
            Mesh mesh = new Mesh();
            mesh.vertices = vertexList.ToArray();
            mesh.triangles = triangleList.ToArray();
            return mesh;
        }

        public static Mesh CreateRing(float inner, float outer, int numOfPoints)
        {
            float angleStep = 360.0f / numOfPoints;
            List<Vector3> vertexList = new List<Vector3>();
            List<int> triangleList = new List<int>();
            
            Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, angleStep);
            
            vertexList.Add(new Vector3(0.0f, inner, 0.0f));
            vertexList.Add(new Vector3(0.0f, outer, 0.0f));

            for (int i = 0; i < numOfPoints-1;i++)
            {
                int innerIndex = vertexList.Count - 2;
                int outerIndex = vertexList.Count - 1;

     
                vertexList.Add(quaternion * vertexList[vertexList.Count - 2]);
                vertexList.Add(quaternion * vertexList[vertexList.Count - 1]);
                
                int nextInnerIndex = vertexList.Count - 2;
                int nextOuterIndex = vertexList.Count - 1;

                triangleList.Add(outerIndex);                     
                triangleList.Add(innerIndex);
                triangleList.Add(nextOuterIndex);
                
                triangleList.Add(outerIndex);                     
                triangleList.Add(nextInnerIndex);
                triangleList.Add(innerIndex);
                
            }
            Mesh mesh = new Mesh();
            mesh.vertices = vertexList.ToArray();
            mesh.triangles = triangleList.ToArray();
            return mesh;
        }

        
       

       

        public static Material _MaterialPlane = null;
        public static Material GetMaterialPlane()
        {
            if (_MaterialPlane == null)
            {
                _MaterialPlane = new Material(Shader.Find("GUI/Text Shader"));
                _MaterialPlane.SetColor("_TintColor", new Color(255, 255, 255, 100));
                _MaterialPlane.SetTexture("_MainTex",
                    AssetManager.Instance.terrainGridProjectorGO.GetComponent<Light>().cookie);
                _MaterialPlane.SetTextureScale("_MainTex", new Vector2(1.0f, 1.0f));
                _MaterialPlane.SetTextureOffset("_MainTex", new Vector2(0f, .5f));
            }

            return _MaterialPlane;
        }
     
    }
}