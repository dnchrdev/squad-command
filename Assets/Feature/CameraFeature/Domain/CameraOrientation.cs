using Feature.Shared.Math;
using UnityEngine;

namespace Feature.CameraFeature.Domain
{
    public class CameraOrientation
    {
        public Quaternion Target { get; private set; } = Quaternion.identity;
        public Quaternion Current { get; private set; } = Quaternion.identity;

        public void SetTarget(Vector2 delta, float changeFactor, float minPitch, float maxPitch)
        {
            float targetYaw = Target.eulerAngles.y + delta.x * changeFactor;
            float targetPitch = Target.eulerAngles.x - delta.y * changeFactor;
            var raw = Quaternion.Euler(targetPitch, targetYaw, 0f);
            Target = ClampPitch(raw, minPitch, maxPitch);
        }

        public void ApplySmoothing(float dt, float smoothing)
        {
            Current = MathExtensions.TranslationWithSmoothing(Current, Target, dt, smoothing);
        }

        public void Reset(Quaternion rotation)
        {
            Target = rotation;
            Current = rotation;
        }

        private Quaternion ClampPitch(Quaternion rotation, float minPitch, float maxPitch)
        {
            var euler = rotation.eulerAngles;
            var clampedPitch = Mathf.Clamp(NormalizePitch(euler.x), minPitch, maxPitch);
            return Quaternion.Euler(clampedPitch, euler.y, 0f);
        }

        private float NormalizePitch(float angle)
            => angle > 180f ? angle - 360f : angle;
    }
}