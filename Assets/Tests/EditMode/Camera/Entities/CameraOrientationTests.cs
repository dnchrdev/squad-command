using Features.Camera.Entities;
using NUnit.Framework;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.Camera.Entities
{
    public class CameraOrientationTests
    {
        private const float CHANGE_FACTOR = 2f;
        private const float MIN_PITCH = -60f;
        private const float MAX_PITCH = 60f;
        private const float DT = 0.1f;
        private const float SMOOTHING = 10f;

        private CameraOrientation _orientation;

        [SetUp]
        public void SetUp() => _orientation = new CameraOrientation();

        [Test]
        public void SetTarget_HorizontalDelta_AddsYaw()
        {
            _orientation.SetTarget(new Vector2(10f, 0f), CHANGE_FACTOR, MIN_PITCH, MAX_PITCH);

            QuaternionAssert.AreEqual(Quaternion.Euler(0f, 20f, 0f), _orientation.Target);
        }

        [Test]
        public void SetTarget_DownwardDelta_RaisesPitch()
        {
            _orientation.SetTarget(new Vector2(0f, -10f), CHANGE_FACTOR, MIN_PITCH, MAX_PITCH);

            QuaternionAssert.AreEqual(Quaternion.Euler(20f, 0f, 0f), _orientation.Target);
        }

        [Test]
        public void SetTarget_PitchAboveMax_ClampsToMax()
        {
            _orientation.SetTarget(new Vector2(0f, -35f), CHANGE_FACTOR, MIN_PITCH, MAX_PITCH);

            QuaternionAssert.AreEqual(Quaternion.Euler(MAX_PITCH, 0f, 0f), _orientation.Target);
        }

        [Test]
        public void SetTarget_PitchBelowMin_ClampsToMin()
        {
            _orientation.SetTarget(new Vector2(0f, 35f), CHANGE_FACTOR, MIN_PITCH, MAX_PITCH);

            QuaternionAssert.AreEqual(Quaternion.Euler(MIN_PITCH, 0f, 0f), _orientation.Target);
        }

        [Test]
        public void SetTarget_DoesNotChangeCurrent()
        {
            _orientation.SetTarget(new Vector2(10f, 0f), CHANGE_FACTOR, MIN_PITCH, MAX_PITCH);

            QuaternionAssert.AreEqual(Quaternion.identity, _orientation.Current);
        }

        [Test]
        public void ApplySmoothing_RotatesCurrentPartWayToTarget()
        {
            _orientation.SetTarget(new Vector2(10f, 0f), CHANGE_FACTOR, MIN_PITCH, MAX_PITCH);

            _orientation.ApplySmoothing(DT, SMOOTHING);

            QuaternionAssert.AreEqual(Quaternion.Euler(0f, 12.6424112f, 0f), _orientation.Current);
        }

        [Test]
        public void Reset_SetsTargetAndCurrent()
        {
            var rotation = Quaternion.Euler(30f, 45f, 0f);

            _orientation.Reset(rotation);

            QuaternionAssert.AreEqual(rotation, _orientation.Target);
            QuaternionAssert.AreEqual(rotation, _orientation.Current);
        }
    }
}
