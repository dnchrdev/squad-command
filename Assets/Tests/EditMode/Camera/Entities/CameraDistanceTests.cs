using Features.Camera.Entities;
using NUnit.Framework;

namespace Tests.EditMode.Camera.Entities
{
    public class CameraDistanceTests
    {
        private const float START_DISTANCE = 25f;
        private const float CHANGE_FACTOR = 3f;
        private const float MIN_DISTANCE = 5f;
        private const float MAX_DISTANCE = 50f;
        private const float DT = 0.1f;
        private const float SMOOTHING = 10f;
        private const float TOLERANCE = 0.0001f;

        private CameraDistance _distance;

        [SetUp]
        public void SetUp()
        {
            _distance = new CameraDistance();
            _distance.Reset(START_DISTANCE);
        }

        [Test]
        public void AddDistance_ZoomInsideBounds_ChangesTargetByZoomTimesFactor()
        {
            _distance.AddDistance(2f, CHANGE_FACTOR, MIN_DISTANCE, MAX_DISTANCE);

            Assert.That(_distance.Target, Is.EqualTo(19f).Within(TOLERANCE));
        }

        [Test]
        public void AddDistance_ZoomPastMax_ClampsToMax()
        {
            _distance.AddDistance(-20f, CHANGE_FACTOR, MIN_DISTANCE, MAX_DISTANCE);

            Assert.That(_distance.Target, Is.EqualTo(MAX_DISTANCE).Within(TOLERANCE));
        }

        [Test]
        public void AddDistance_ZoomPastMin_ClampsToMin()
        {
            _distance.AddDistance(10f, CHANGE_FACTOR, MIN_DISTANCE, MAX_DISTANCE);

            Assert.That(_distance.Target, Is.EqualTo(MIN_DISTANCE).Within(TOLERANCE));
        }

        [Test]
        public void AddDistance_DoesNotChangeCurrent()
        {
            _distance.AddDistance(2f, CHANGE_FACTOR, MIN_DISTANCE, MAX_DISTANCE);

            Assert.That(_distance.Current, Is.EqualTo(START_DISTANCE).Within(TOLERANCE));
        }

        [Test]
        public void ApplySmoothing_MovesCurrentPartWayToTarget()
        {
            _distance.AddDistance(2f, CHANGE_FACTOR, MIN_DISTANCE, MAX_DISTANCE);

            _distance.ApplySmoothing(DT, SMOOTHING);

            Assert.That(_distance.Current, Is.EqualTo(21.2072766f).Within(TOLERANCE));
        }

        [Test]
        public void Reset_SetsTargetAndCurrent()
        {
            _distance.AddDistance(2f, CHANGE_FACTOR, MIN_DISTANCE, MAX_DISTANCE);

            _distance.Reset(40f);

            Assert.That(_distance.Target, Is.EqualTo(40f).Within(TOLERANCE));
            Assert.That(_distance.Current, Is.EqualTo(40f).Within(TOLERANCE));
        }
    }
}
