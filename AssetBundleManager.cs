using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TrackEdit
{
    public class AssetBundleManager
    {
        private readonly Main _main;
        public readonly Material MaterialPlane;
        public readonly GameObject NodeGo;
        public readonly GameObject NodeRotateGo;
       // public readonly GameObject UiContainerWindowGo;
        //public readonly GameObject UiHeaderPanelGo;

        private AssetBundle assetBundle;

        public AssetBundleManager(Main main)
        {

            _main = main;
            var dsc = System.IO.Path.DirectorySeparatorChar;
            assetBundle = AssetBundle.LoadFromFile(_main.Path + dsc + "trackedit");


            MaterialPlane = new Material(Shader.Find("GUI/Text Shader"));
            MaterialPlane.SetColor("_TintColor", new Color(255, 255, 255, 100));
            MaterialPlane.SetTexture("_MainTex",
                AssetManager.Instance.terrainGridProjectorGO.GetComponent<Light>().cookie);
            MaterialPlane.SetTextureScale("_MainTex", new Vector2(1.0f, 1.0f));
            MaterialPlane.SetTextureOffset("_MainTex", new Vector2(0f, .5f));
            
            Material nodeMaterial =  new Material(Shader.Find("UI/Default"));
            nodeMaterial.SetColor("_TintColor", new Color(255, 255, 255, 100));
            
            NodeGo = new GameObject();
            NodeGo.layer = LayerMasks.COASTER_TRACKS;
            SphereCollider sphereCollider =  NodeGo.AddComponent<SphereCollider>();
            sphereCollider.center = new Vector3(0, 0.5f, 0.0f);
            sphereCollider.radius = .1f;
                
            GameObject child = new GameObject();

            MeshFilter meshFilter = child.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = CreateCircle(15);
            MeshRenderer meshRenderer = child.AddComponent<MeshRenderer>();
            meshRenderer.material = nodeMaterial;
            //meshRenderer.material.SetColor("_TintColor", new Color(255,255,255,100));

            LineRenderer lineRenderer = child.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 3;
            lineRenderer.useWorldSpace = true;
            lineRenderer.material = nodeMaterial;
            lineRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.Camera;
            lineRenderer.allowOcclusionWhenDynamic = true;
            lineRenderer.startWidth = .1f;
            lineRenderer.startWidth = .1f;
            child.transform.position = new Vector3(0,.5f,0);
            child.transform.parent = NodeGo.transform;
            child.name = "item";


            NodeRotateGo = assetBundle.LoadAsset<GameObject>("Node_Rotate");
            NodeGo = assetBundle.LoadAsset<GameObject>("Node");
            NodeGo.layer = LayerMasks.COASTER_TRACKS;

            
        }

    

        public static Mesh CreateCircle(int numOfPoints)
        {
            float angleStep = 360.0f / (float)numOfPoints;
            List<Vector3> vertexList = new List<Vector3>();
            List<int> triangleList = new List<int>();
            Quaternion quaternion = Quaternion.Euler(0.0f, 0.0f, angleStep);
            // Make first triangle.
            vertexList.Add(new Vector3(0.0f, 0.0f, 0.0f));  // 1. Circle center.
            vertexList.Add(new Vector3(0.0f, 0.1f, 0.0f));  // 2. First vertex on circle outline (radius = 0.5f)
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


    }
}