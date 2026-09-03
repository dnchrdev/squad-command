using UnityEngine;

namespace Feature.Command.Domain
{
    public class Formation
    {
        private const float MIN_DRAG_FOR_ROTATION = 1f;

        public Vector2 Center { get; private set; }
        public Vector2 Forward { get; private set; } = Vector2.up;
        public Vector2 Right { get; private set; } = Vector2.right;
        public Vector2 Size { get; private set; }

        public Formation(Vector2 unitsCenter, Vector2 dragStart)
        {
            Center = dragStart;
            SetDirection((dragStart - unitsCenter).normalized);
        }

        public void UpdateDrag(Vector2 dragStart, Vector2 dragEnd)
        {
            Center = dragStart;
            if (Vector2.Distance(dragStart, dragEnd) > MIN_DRAG_FOR_ROTATION)
                SetDirection((dragStart - dragEnd).normalized);
            Resize(dragStart, dragEnd);
        }

        private void SetDirection(Vector2 forward)
        {
            if (forward == Vector2.zero) return;
            Forward = forward;
            Right = new Vector2(forward.y, -forward.x);
        }

        private void Resize(Vector2 a, Vector2 b)
        {
            var delta = a - b;
            Size = new Vector2(
                Mathf.Abs(Vector2.Dot(delta, Right)),
                Mathf.Abs(Vector2.Dot(delta, Forward)));
        }
    }
}