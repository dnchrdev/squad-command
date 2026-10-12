using Features.Camera.Entities;
using Features.Camera.UseCases;
using NUnit.Framework;
using Tests.EditMode.Camera.UseCases.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.Camera.UseCases
{
    public class CameraPositionUseCaseTests
    {
        private const float DT = 0.1f;
        private const float LARGE_DT = 100f;

        private CameraPosition _position;
        private StubCameraOrientation _orientation;
        private SpyCameraPosition _cameraPosition;
        private CameraPositionUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _position = new CameraPosition();
            _orientation = new StubCameraOrientation();
            _cameraPosition = new SpyCameraPosition();
            _useCase = new CameraPositionUseCase(_position, CameraSettingsFactory.Create(), _cameraPosition, _orientation);
        }

        [Test]
        public void Tick_SendsSmoothedPositionToCamera()
        {
            _useCase.Tick(DT, new Vector2(0f, 1f));

            VectorAssert.AreEqual(new Vector3(0f, 0f, -1.2642411f), _cameraPosition.LastPosition);
        }

        [Test]
        public void Tick_ZeroDelta_KeepsPosition()
        {
            var start = new Vector3(1f, 0f, 1f);
            _position.Reset(start);

            _useCase.Tick(DT, Vector2.zero);

            VectorAssert.AreEqual(start, _cameraPosition.LastPosition);
        }

        [Test]
        public void Tick_UsesCameraOrientationForDirection()
        {
            _orientation.Forward = Vector3.right;
            _orientation.Right = Vector3.back;

            _useCase.Tick(LARGE_DT, new Vector2(0f, 1f));

            VectorAssert.AreEqual(new Vector3(-2f, 0f, 0f), _cameraPosition.LastPosition);
        }
    }
}
