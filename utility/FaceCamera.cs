using UnityEngine;

namespace TrackEdit
{
    public class FaceCamera : MonoBehaviour
    {
        private void Update()
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                transform.LookAt(Camera.main.transform, Vector3.up);
            }
        }
    }
}