using Features.Camera.Entities;
using Features.Camera.UseCases;
using NUnit.Framework;
using Tests.EditMode.Camera.UseCases.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.Camera.UseCases
{
    public class CameraStartUseCaseTests
    {
        private const float TOLERANCE = 0.0001f;

        private CameraOrientation _orientation;
        private CameraPosition _position;
        private CameraDistance _distance;
        private SpyCameraRotation _cameraRotation;
        private SpyCameraPosition _cameraPosition;
        private SpyCameraBoom _cameraBoom;
        private CameraStartUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _orientation = new CameraOrientation();
            _position = new CameraPosition();
            _distance = new CameraDistance();
            _cameraRotation = new SpyCameraRotation();
            _cameraPosition = new SpyCameraPosition();
            _cameraBoom = new SpyCameraBoom();
            _useCase = new CameraStartUseCase(_orientation, _position, _distance, CameraSettingsFactory.Create(),
                _cameraRotation, _cameraPosition, _cameraBoom);

            _orientation.Reset(Quaternion.Euler(30f, 45f, 0f));
            _position.Reset(new Vector3(5f, 0f, 5f));
            _distance.Reset(40f);
        }

        [Test]
        public void Start_ResetsCameraState()
        {
            _useCase.Start();

            QuaternionAssert.AreEqual(Quaternion.identity, _orientation.Target);
            VectorAssert.AreEqual(Vector3.zero, _position.Target);
            Assert.That(_distance.Target, Is.EqualTo(CameraSettingsFactory.START_DISTANCE).Within(TOLERANCE));
        }

        [Test]
        public void Start_SendsInitialStateToCamera()
        {
            _useCase.Start();

            QuaternionAssert.AreEqual(Quaternion.identity, _cameraRotation.LastRotation);
            VectorAssert.AreEqual(Vector3.zero, _cameraPosition.LastPosition);
            Assert.That(_cameraBoom.LastDistance, Is.EqualTo(CameraSettingsFactory.START_DISTANCE).Within(TOLERANCE));
        }
    }
}
