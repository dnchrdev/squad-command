using Features.Camera.Entities;
using Features.Camera.UseCases;
using NUnit.Framework;
using Tests.EditMode.Camera.UseCases.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.Camera.UseCases
{
    public class CameraRotationUseCaseTests
    {
        private const float DT = 0.1f;
        private const float LARGE_DT = 100f;

        private SpyCameraRotation _cameraRotation;
        private CameraRotationUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _cameraRotation = new SpyCameraRotation();
            _useCase = new CameraRotationUseCase(new CameraOrientation(), CameraSettingsFactory.Create(), _cameraRotation);
        }

        [Test]
        public void Tick_SendsSmoothedRotationToCamera()
        {
            _useCase.Tick(DT, new Vector2(10f, 0f));

            QuaternionAssert.AreEqual(Quaternion.Euler(0f, 12.6424112f, 0f), _cameraRotation.LastRotation);
        }

        [Test]
        public void Tick_PitchPastMax_ClampsToSettingsMax()
        {
            _useCase.Tick(LARGE_DT, new Vector2(0f, -44f));

            QuaternionAssert.AreEqual(Quaternion.Euler(CameraSettingsFactory.MAX_PITCH, 0f, 0f), _cameraRotation.LastRotation);
        }
    }
}
