using Feature.Shared.Math;
using UnityEngine;

namespace Feature.CameraFeature.Domain
{
    public class CameraPosition
    {
        public Vector3 Target { get; private set; }
        public Vector3 Current { get; private set; }

        public void SetTarget(Vector2 delta, Vector3 forward, Vector3 right, float changeFactor)
        {
            var planarForward = Vector3.ProjectOnPlane(forward, Vector3.up).normalized;
            var planarRight = Vector3.ProjectOnPlane(right, Vector3.up).normalized;
            var offset = planarForward * -delta.y + planarRight * -delta.x;
            Target += offset * changeFactor;
        }

        public void ApplySmoothing(float dt, float smoothing)
        {
            Current = MathExtensions.TranslationWithSmoothing(Current, Target, dt, smoothing);
        }

        public void Reset(Vector3 position)
        {
            Target = position;
            Current = position;
        }
    }
}