using UnityEngine;

namespace Feature.CameraFeature.Application
{
    public readonly struct CameraInputResolution
    {
        public readonly Vector2 MoveDelta;
        public readonly Vector2 RotateDelta;

        public CameraInputResolution(Vector2 moveDelta, Vector2 rotateDelta)
        {
            MoveDelta = moveDelta;
            RotateDelta = rotateDelta;
        }
    }

    public class CameraInputResolverUseCase
    {
        public CameraInputResolution Resolve(bool isDragging, bool isRotating, Vector2 pointerDelta)
        {
            if (isDragging)
                return new CameraInputResolution(pointerDelta, Vector2.zero);

            if (isRotating)
                return new CameraInputResolution(Vector2.zero, pointerDelta);

            return new CameraInputResolution(Vector2.zero, Vector2.zero);
        }
    }
}