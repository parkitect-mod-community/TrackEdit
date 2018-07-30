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

        public static Mesh CreateRing(float inner, float outter, int numOfPoints)
        {
            float angleStep = 360.0f / numOfPoints;
            List<Vector3> vertexList = new List<Vector3>();
            List<int> triangleList = new List<int>();
            
            Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, angleStep);
            
            vertexList.Add(new Vector3(0.0f, inner, 0.0f));
            vertexList.Add(new Vector3(0.0f, outter, 0.0f));

            for (int i = 0; i < numOfPoints-1;i++)
            {
                int innerIndex = vertexList.Count - 2;
                int outterIndex = vertexList.Count - 1;

     
                vertexList.Add(quaternion * vertexList[vertexList.Count - 1]);
                vertexList.Add(quaternion * vertexList[vertexList.Count - 2]);
                
                int nexinnerIndex = vertexList.Count - 2;
                int nextoutterIndex = vertexList.Count - 1;

                triangleList.Add(outterIndex);                     
                triangleList.Add(innerIndex);
                triangleList.Add(nexinnerIndex);
                
                triangleList.Add(outterIndex);                     
                triangleList.Add(nextoutterIndex);
                triangleList.Add(nexinnerIndex);
                
            }
            Mesh mesh = new Mesh();
            mesh.vertices = vertexList.ToArray();
            mesh.triangles = triangleList.ToArray();
            return mesh;
        }

        
        private static Mesh _nodeCircleMesh = null;
        private static Material _nodeMaterial = null;
        public static GameObject CreateNode()
        {
            if(_nodeCircleMesh == null)
                _nodeCircleMesh = CreateCircle(0.1f,10);;
            if (_nodeMaterial == null)
            {
                _nodeMaterial =  new Material(Shader.Find("UI/Default"));
                _nodeMaterial.SetColor("_TintColor", new Color(255, 255, 255, 100));
            }
           
            GameObject result = new GameObject();
            result.layer = LayerMasks.COASTER_TRACKS;
            
            SphereCollider sphereCollider =  result.AddComponent<SphereCollider>();
            sphereCollider.center = new Vector3(0, 0.5f, 0.0f);
            sphereCollider.radius = .1f;
                
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
            lineRenderer.startWidth = .1f;
            
            child.transform.position = new Vector3(0,.5f,0);
            child.transform.parent = result.transform;
            child.name = "item";

            return result;
        }

        private static Mesh _nodeRotateMesh = null;
        private static Material _ringMaterial = null;
        private static Material _ringAngleMaterial;
        public static GameObject CreateNodeRotate()
        {
            if (_nodeRotateMesh == null)
            {
                CombineInstance[] combine = new CombineInstance[2];
                combine[0].mesh = CreateCircle(0.1f, 10);
                combine[0].transform = Matrix4x4.Translate(new Vector3(0,1.5f,0));
                
                Mesh ring = CreateRing(1.5f - .02f, 1.5f + .02f, 10);
                combine[1].mesh = ring;
                combine[1].transform = Matrix4x4.identity;
               
                _nodeRotateMesh = new Mesh();
                _nodeRotateMesh.CombineMeshes(combine);
            }

            if (_ringMaterial == null)
            {
                _ringMaterial =  new Material(Shader.Find("UI/Default"));
                _ringMaterial.SetColor("_TintColor", new Color(255, 255, 255, 100));
            }

            if (_ringAngleMaterial == null)
            {
                _ringAngleMaterial = new Material(Shader.Find("GUI/Text Shader"));
            }
            
            GameObject result = new GameObject();

            GameObject rotateGO = new GameObject("Rotate");
            rotateGO.AddComponent<MeshFilter>().sharedMesh = _nodeRotateMesh;
            rotateGO.AddComponent<MeshRenderer>().sharedMaterial = _ringMaterial;

            SphereCollider sphereCollider = rotateGO.AddComponent<SphereCollider>();
            sphereCollider.center = new Vector3(0,1.5f,0);
            sphereCollider.radius = .2f;

            rotateGO.transform.parent = result.transform;
            
            GameObject AngleGo = new GameObject("Angle");
            AngleGo.AddComponent<MeshRenderer>().sharedMaterial = _ringAngleMaterial;
            TextMesh textMesh = AngleGo.AddComponent<TextMesh>();
            textMesh.characterSize = .03f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Left;
            textMesh.fontSize = 125;

            AngleGo.transform.parent = result.transform;
            
            AngleGo.transform.localScale = new Vector3(-1,1,1);
            AngleGo.transform.position = new Vector3(.827f,1.769f,0f);
            
            
            return result;
        }

     
    }
}