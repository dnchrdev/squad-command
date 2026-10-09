using UnityEngine;

namespace Features.Camera.Entities
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
}