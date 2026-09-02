using Feature.Shared.Math;
using UnityEngine;

namespace Feature.CameraFeature.Domain
{
    public class CameraDistance
    {
        public float Target { get; private set; }
        public float Current { get; private set; }

        public void AddDistance(float zoomValue, float changeFactor, float min, float max)
        {
            Target = Mathf.Clamp(Target - zoomValue * changeFactor, min, max);
        }

        public void ApplySmoothing(float dt, float smoothing)
        {
            Current = MathExtensions.TranslationWithSmoothing(Current, Target, dt, smoothing);
        }

        public void Reset(float distance)
        {
            Target = distance;
            Current = distance;
        }
    }
}