using UnityEngine;

namespace Feature.CameraFeature.Domain
{
    public static class CameraRotationMath
    {
        public static Quaternion ClampPitch(Quaternion rotation, float minPitch, float maxPitch)
        {
            var euler = rotation.eulerAngles;
            var clampedPitch = Mathf.Clamp(NormalizePitch(euler.x), minPitch, maxPitch);
            return Quaternion.Euler(clampedPitch, euler.y, 0f);
        }
        
        private static float NormalizePitch(float angle)
            => angle > 180f ? angle - 360f : angle;
    }
}