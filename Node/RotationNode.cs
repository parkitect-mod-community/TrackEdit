using TrackEdit.Node;
using UnityEngine;

namespace TrackEdit
{
    public class RotationNode : BaseNode
    {

        public TrackSegmentHandler Handler { get; set; }
        private Transform _root;
        private TextMesh _text;
        private Transform _textTransform;

        public float Rotation { get; set; }


        public override void OnNotifySegmentChange()
        {
            
        }

        protected override void Update()
        {
          /*  _root.rotation = Quaternion.LookRotation(Handler.TrackSegment.getTangentPoint(1f));

            transform.localEulerAngles = new Vector3(0, 0, Handler.TrackSegment.totalRotation);
            _text.text = Handler.TrackSegment.totalRotation % 360 + "\u00B0";
            _textTransform.LookAt(Camera.main.transform, Vector3.up);*/
        }

        protected override void Awake()
        {
          //  _root = transform.parent;
        //    _textTransform = transform.Find("Angle");
        //    _text = _textTransform.GetComponent<TextMesh>();
        }


        public override void OnPressed(RaycastHit hit)
        {
        }

        public override void OnHold()
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

        public override void OnRelease()
        {
        }


        private static Mesh _nodeRotateMesh = null;
        private static Material _ringMaterial = null;
        private static Material _ringAngleMaterial;
        private static readonly int TintColor = Shader.PropertyToID("_TintColor");

        public static RotationNode Build(TrackSegmentHandler handler)
        {
            if (_nodeRotateMesh == null)
            {
                CombineInstance[] combine = new CombineInstance[2];
                combine[0].mesh = GameObjectUtility.CreateCircle(0.1f, 10);
                combine[0].transform = Matrix4x4.Translate(new Vector3(0, 1.5f, 0));

                Mesh ring = GameObjectUtility.CreateRing(1.5f - .02f, 1.5f + .02f, 10);
                combine[1].mesh = ring;
                combine[1].transform = Matrix4x4.identity;

                _nodeRotateMesh = new Mesh();
                _nodeRotateMesh.CombineMeshes(combine);
            }

            if (_ringMaterial == null)
            {
                _ringMaterial = new Material(Shader.Find("UI/Default"));
                _ringMaterial.SetColor(TintColor, new Color(255, 255, 255, 100));
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
            sphereCollider.center = new Vector3(0, 1.5f, 0);
            sphereCollider.radius = .2f;

            rotateGO.transform.parent = result.transform;

            GameObject angleGo = new GameObject("Angle");
            angleGo.AddComponent<MeshRenderer>().sharedMaterial = _ringAngleMaterial;
            TextMesh textMesh = angleGo.AddComponent<TextMesh>();
            textMesh.characterSize = .03f;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Left;
            textMesh.fontSize = 125;

            angleGo.transform.parent = result.transform;
            angleGo.transform.localScale = new Vector3(-1, 1, 1);
            angleGo.transform.position = new Vector3(.827f, 1.769f, 0f);


            RotationNode rotationNode = result.AddComponent<RotationNode>();
            rotationNode.Handler = handler;
            return rotationNode;
        }
    }
}