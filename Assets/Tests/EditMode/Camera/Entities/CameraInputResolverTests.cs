using Features.Camera.Entities;
using NUnit.Framework;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.Camera.Entities
{
    public class CameraInputResolverTests
    {
        private static readonly Vector2 PointerDelta = new Vector2(3f, -4f);

        [Test]
        public void Resolve_Dragging_ReturnsDeltaAsMove()
        {
            var result = CameraInputResolver.Resolve(true, false, PointerDelta);

            VectorAssert.AreEqual(PointerDelta, result.MoveDelta);
            VectorAssert.AreEqual(Vector2.zero, result.RotateDelta);
        }

        [Test]
        public void Resolve_Rotating_ReturnsDeltaAsRotate()
        {
            var result = CameraInputResolver.Resolve(false, true, PointerDelta);

            VectorAssert.AreEqual(Vector2.zero, result.MoveDelta);
            VectorAssert.AreEqual(PointerDelta, result.RotateDelta);
        }

        [Test]
        public void Resolve_DraggingAndRotating_PrefersMove()
        {
            var result = CameraInputResolver.Resolve(true, true, PointerDelta);

            VectorAssert.AreEqual(PointerDelta, result.MoveDelta);
            VectorAssert.AreEqual(Vector2.zero, result.RotateDelta);
        }

        [Test]
        public void Resolve_Idle_ReturnsZeroDeltas()
        {
            var result = CameraInputResolver.Resolve(false, false, PointerDelta);

            VectorAssert.AreEqual(Vector2.zero, result.MoveDelta);
            VectorAssert.AreEqual(Vector2.zero, result.RotateDelta);
        }
    }
}
