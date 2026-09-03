using UnityEngine;

namespace Feature.Command.Adapter
{
    public class LinkerGeometry
    {
        public static float ComputeZRotation(Vector2 screenDelta)
        {
            float angle = Vector2.Angle(Vector2.up, screenDelta);
            bool isNegative = Vector2.Dot(screenDelta, Vector2.right) > 0;
            return angle * (isNegative ? -1 : 1);
        }

        public static float ComputeLength(Vector2 screenDelta, Vector2 canvasScaleFactor)
        {
            float dy = Vector2.Dot(screenDelta, Vector2.up) * canvasScaleFactor.y;
            float dx = Vector2.Dot(screenDelta, Vector2.right) * canvasScaleFactor.x;
            return new Vector2(dy, dx).magnitude;
        }
    }
}