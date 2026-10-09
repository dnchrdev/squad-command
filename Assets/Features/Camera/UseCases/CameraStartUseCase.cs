using Features.Camera.Entities;
using Features.Camera.UseCases.Ports;
using Features.Shared.Lifecycle;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Features.Camera.UseCases
{
    public sealed class CameraStartUseCase : ICameraStart
    {
        private readonly CameraOrientation _orientation;
        private readonly CameraPosition _position;
        private readonly CameraDistance _distance;
        private readonly CameraSettings _settings;
        private readonly ICameraRotation _cameraRotation;
        private readonly ICameraPosition _cameraPosition;
        private readonly ICameraBoom _cameraBoom;

        public CameraStartUseCase(
            CameraOrientation orientation,
            CameraPosition position,
            CameraDistance distance,
            CameraSettings settings,
            ICameraRotation cameraRotation,
            ICameraPosition cameraPosition,
            ICameraBoom cameraBoom)
        {
            _orientation = orientation;
            _position = position;
            _distance = distance;
            _settings = settings;
            _cameraRotation = cameraRotation;
            _cameraPosition = cameraPosition;
            _cameraBoom = cameraBoom;
        }

        public void Start()
        {
            _orientation.Reset(Quaternion.identity);
            _position.Reset(Vector3.zero);
            _distance.Reset(_settings.StartDistance);

            _cameraRotation.SetRotation(_orientation.Current);
            _cameraPosition.SetPosition(_position.Current);
            _cameraBoom.SetDistance(_distance.Current);
        }
    }
}