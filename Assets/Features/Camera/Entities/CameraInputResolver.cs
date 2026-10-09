using UnityEngine;

namespace Features.Camera.Entities
{
    public static class CameraInputResolver
    {
        public static CameraInputResolution Resolve(bool isDragging, bool isRotating, Vector2 pointerDelta)
        {
            if (isDragging)
                return new CameraInputResolution(pointerDelta, Vector2.zero);

            if (isRotating)
                return new CameraInputResolution(Vector2.zero, pointerDelta);

            return new CameraInputResolution(Vector2.zero, Vector2.zero);
        }
    }
}