using Features.Camera.Entities;
using NUnit.Framework;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.Camera.Entities
{
    public class CameraPositionTests
    {
        private const float CHANGE_FACTOR = 2f;
        private const float DT = 0.1f;
        private const float SMOOTHING = 10f;

        private CameraPosition _position;

        [SetUp]
        public void SetUp() => _position = new CameraPosition();

        [Test]
        public void SetTarget_UpDelta_MovesBackAlongForward()
        {
            _position.SetTarget(new Vector2(0f, 1f), Vector3.forward, Vector3.right, CHANGE_FACTOR);

            VectorAssert.AreEqual(new Vector3(0f, 0f, -2f), _position.Target);
        }

        [Test]
        public void SetTarget_RightDelta_MovesAlongNegativeRight()
        {
            _position.SetTarget(new Vector2(1f, 0f), Vector3.forward, Vector3.right, CHANGE_FACTOR);

            VectorAssert.AreEqual(new Vector3(-2f, 0f, 0f), _position.Target);
        }

        [Test]
        public void SetTarget_TiltedForward_StaysOnGroundPlane()
        {
            var tiltedForward = new Vector3(0f, -1f, 1f);

            _position.SetTarget(new Vector2(0f, 1f), tiltedForward, Vector3.right, CHANGE_FACTOR);

            VectorAssert.AreEqual(new Vector3(0f, 0f, -2f), _position.Target);
        }

        [Test]
        public void SetTarget_CalledTwice_Accumulates()
        {
            _position.SetTarget(new Vector2(0f, 1f), Vector3.forward, Vector3.right, CHANGE_FACTOR);
            _position.SetTarget(new Vector2(0f, 1f), Vector3.forward, Vector3.right, CHANGE_FACTOR);

            VectorAssert.AreEqual(new Vector3(0f, 0f, -4f), _position.Target);
        }

        [Test]
        public void ApplySmoothing_MovesCurrentPartWayToTarget()
        {
            _position.SetTarget(new Vector2(0f, 1f), Vector3.forward, Vector3.right, CHANGE_FACTOR);

            _position.ApplySmoothing(DT, SMOOTHING);

            VectorAssert.AreEqual(new Vector3(0f, 0f, -1.2642411f), _position.Current);
        }

        [Test]
        public void Reset_SetsTargetAndCurrent()
        {
            var point = new Vector3(5f, 0f, 7f);

            _position.Reset(point);

            VectorAssert.AreEqual(point, _position.Target);
            VectorAssert.AreEqual(point, _position.Current);
        }
    }
}
