using UnityEngine;

namespace TrackEdit
{
    public static class MathHelper
    {
        public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
        {
            return Mathf.Atan2(
                       Vector3.Dot(n, Vector3.Cross(v1, v2)),
                       Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;
        }

        public static Vector3 HitPlane(Vector3 origin, Vector3 normal, Ray ray)
        {
            return Vector3.zero;
        }
    }
}