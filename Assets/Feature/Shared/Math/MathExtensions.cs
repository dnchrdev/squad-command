using UnityEngine;

namespace Feature.Shared.Math
{
    public static class MathExtensions
    {
        public static float TranslationWithSmoothing(float current, float target, float dt, float smoothing)
        {
            return Mathf.Lerp(current, target, 1f - Mathf.Exp(-smoothing * dt));
        }
        
        public static Vector3 TranslationWithSmoothing(Vector3 current, Vector3 target, float dt, float smoothing)
        {
            return Vector3.Lerp(current, target, 1f - Mathf.Exp(-smoothing * dt));
        }
        
        public static Quaternion TranslationWithSmoothing(Quaternion current, Quaternion target, float dt, float smoothing)
        {
            return Quaternion.Slerp(current, target, 1f - Mathf.Exp(-smoothing * dt));
        }
    }
}