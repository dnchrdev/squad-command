using Features.Camera.Entities;
using Features.Camera.UseCases.Ports;
using Vector2 = UnityEngine.Vector2;

namespace Features.Camera.UseCases
{
    public sealed class CameraPositionUseCase
    {
        private readonly CameraPosition _position;
        private readonly CameraSettings _settings;
        private readonly ICameraPosition _cameraPosition;
        private readonly IReadOnlyCameraOrientation _orientation;

        public CameraPositionUseCase(
            CameraPosition position,
            CameraSettings settings,
            ICameraPosition cameraPosition,
            IReadOnlyCameraOrientation orientation)
        {
            _position = position;
            _settings = settings;
            _cameraPosition = cameraPosition;
            _orientation = orientation;
        }

        public void Tick(float dt, Vector2 moveDelta)
        {
            _position.SetTarget(moveDelta, _orientation.Forward, _orientation.Right, _settings.MoveChangeFactor);
            _position.ApplySmoothing(dt, _settings.MoveSmoothing);
            _cameraPosition.SetPosition(_position.Current);
        }
    }
}