using Features.Camera.Entities;
using Features.Camera.UseCases;
using NUnit.Framework;
using Tests.EditMode.Camera.UseCases.Fakes;

namespace Tests.EditMode.Camera.UseCases
{
    public class CameraBoomUseCaseTests
    {
        private const float DT = 0.1f;
        private const float TOLERANCE = 0.0001f;

        private CameraDistance _distance;
        private SpyCameraBoom _boom;
        private CameraBoomUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _distance = new CameraDistance();
            _distance.Reset(CameraSettingsFactory.START_DISTANCE);
            _boom = new SpyCameraBoom();
            _useCase = new CameraBoomUseCase(_distance, CameraSettingsFactory.Create(), _boom);
        }

        [Test]
        public void AddDistance_ChangesTargetUsingSettings()
        {
            _useCase.AddDistance(2f);

            Assert.That(_distance.Target, Is.EqualTo(19f).Within(TOLERANCE));
        }

        [Test]
        public void AddDistance_DoesNotMoveCamera()
        {
            _useCase.AddDistance(2f);

            Assert.That(_boom.Calls, Is.Zero);
        }

        [Test]
        public void Tick_SendsSmoothedDistanceToBoom()
        {
            _useCase.AddDistance(2f);

            _useCase.Tick(DT);

            Assert.That(_boom.LastDistance, Is.EqualTo(21.2072766f).Within(TOLERANCE));
        }
    }
}
