using Features.Camera.Entities;
using Features.Camera.UseCases.Ports;

namespace Features.Camera.UseCases
{
    public sealed class CameraBoomUseCase
    {
        private readonly CameraDistance _distance;
        private readonly CameraSettings _settings;
        private readonly ICameraBoom _cameraBoom;

        public CameraBoomUseCase(CameraDistance distance, CameraSettings settings, ICameraBoom cameraBoom)
        {
            _distance = distance;
            _settings = settings;
            _cameraBoom = cameraBoom;
        }

        public void AddDistance(float zoomValue)
        {
            _distance.AddDistance(zoomValue, _settings.DistanceChangeFactor, _settings.MinDistance, _settings.MaxDistance);
        }

        public void Tick(float dt)
        {
            _distance.ApplySmoothing(dt, _settings.DistanceSmoothing);
            _cameraBoom.SetDistance(_distance.Current);
        }
    }
}