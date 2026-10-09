using Features.Camera.Entities;
using Features.Camera.UseCases.Ports;
using Vector2 = UnityEngine.Vector2;

namespace Features.Camera.UseCases
{
    public sealed class CameraRotationUseCase
    {
        private readonly CameraOrientation _orientation;
        private readonly CameraSettings _settings;
        private readonly ICameraRotation _cameraRotation;

        public CameraRotationUseCase(CameraOrientation orientation, CameraSettings settings, ICameraRotation cameraRotation)
        {
            _orientation = orientation;
            _settings = settings;
            _cameraRotation = cameraRotation;
        }

        public void Tick(float dt, Vector2 rotateDelta)
        {
            _orientation.SetTarget(rotateDelta, _settings.RotationChangeFactor, _settings.MinPitch, _settings.MaxPitch);
            _orientation.ApplySmoothing(dt, _settings.RotationSmoothing);
            _cameraRotation.SetRotation(_orientation.Current);
        }
    }
}