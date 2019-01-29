/**
* Copyright 2019 Michael Pollind
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using TrackEdit.Node;
using UnityEngine;

namespace TrackEdit
{
    public class RotationNode : BaseNode
    {

        public TrackSegmentHandler Handler { get; set; }


        private GameObject _angleGo;
        private TextMesh _textMesh;
        
        public override void OnNotifySegmentChange()
        {
        }

        protected override void Update()
        {
            if (Handler != null)
            {
                
                Vector3 binormal = Vector3.Cross(Vector3.up, Handler.TrackSegment.getTangentPoint(1f));
                _angleGo.transform.position = transform.position + binormal * 1.5f + Vector3.up * 1.5f;
                _angleGo.transform.LookAt(Camera.main.transform, Vector3.up);
               
                transform.rotation =  Quaternion.LookRotation(Handler.TrackSegment.getTangentPoint(1f)) * Quaternion.Euler(0,0,Handler.TrackSegment.totalRotation);

                _textMesh.text = Handler.TrackSegment.totalRotation % 360 + "\u00B0";
            }
        }

        protected override void Awake()
        {
            _angleGo = new GameObject();
            MeshRenderer meshRenderer = _angleGo.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = AssetManager.Instance.signTextMaterial;
           
            _textMesh = _angleGo.AddComponent<TextMesh>();
            _textMesh.characterSize = .03f;
            _textMesh.anchor = TextAnchor.MiddleCenter;
            _textMesh.alignment = TextAlignment.Left;
            _textMesh.fontSize = 125;
            _textMesh.richText = true;
            
        }
        

        private void OnDestroy()
        {
            if(_angleGo != null)
                Destroy(_angleGo);
        }

        public override void OnHold()
        {
            base.OnHold();
            if (Handler != null)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                var planeNormal = Handler.TrackSegment.getTangentPoint(1.0f);
                var planeCenter = transform.position;

                var l = planeNormal.x * ray.origin.x + planeNormal.y * ray.origin.y + planeNormal.z * ray.origin.z;
                var r = planeNormal.x * planeCenter.x + planeNormal.y * planeCenter.y + planeCenter.z * planeNormal.z;

                var t = (-l + r) / (planeNormal.x * ray.direction.x + planeNormal.y * ray.direction.y +
                                    planeNormal.z * ray.direction.z);

                var loc = ray.origin + ray.direction * t;
                var diff = MathHelper.AngleSigned(Handler.TrackSegment.getNormal(1.0f),
                    Vector3.Normalize(planeCenter - loc), planeNormal);

                Handler.CalculateWithNewTotalRotation(
                    Mathf.Round(diff + Handler.TrackSegment.totalRotation));
                Handler.Invalidate = true;

                var nextSegment = Handler.GetNextSegment(true);

                if (nextSegment != null)
                    nextSegment.Invalidate = true;
            }

        }



        private static Mesh _nodeRotateMesh = null;

        public static readonly float RingRadius = 1.5f;
        public static readonly float HalfRingWidth = .02f;
        
        public static RotationNode Build(TrackSegmentHandler handler)
        {
            if (_nodeRotateMesh == null)
            {
                CombineInstance[] combine = new CombineInstance[2];
                combine[0].mesh = GameObjectUtility.CreateCircle(EmptyNode.NodeRadius, 10);
                combine[0].transform = Matrix4x4.Translate(new Vector3(0, RingRadius, 0));

                Mesh ring = GameObjectUtility.CreateRing(RingRadius - HalfRingWidth , RingRadius + HalfRingWidth , 40);
                combine[1].mesh = ring;
                combine[1].transform = Matrix4x4.identity;

                _nodeRotateMesh = new Mesh();
                _nodeRotateMesh.CombineMeshes(combine);
            }

            GameObject result = new GameObject();

            result.AddComponent<MeshFilter>().sharedMesh = _nodeRotateMesh;
            result.AddComponent<MeshRenderer>().sharedMaterial = EmptyNode.DefaultNodeMaterial();
     
            SphereCollider sphereCollider = result.AddComponent<SphereCollider>();
            sphereCollider.center = new Vector3(0, RingRadius, 0);
            sphereCollider.radius = EmptyNode.NodeRadius;

            RotationNode rotationNode = result.AddComponent<RotationNode>();
            rotationNode.Handler = handler;
            return rotationNode;
        }
    }
}