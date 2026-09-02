using Feature.CameraFeature.Application.Interfaces;
using Feature.CameraFeature.Domain;
using Feature.CameraFeature.Infrastructure;
using Zenject;

namespace Feature.CameraFeature.Application
{
    public class CameraBoomUseCase
    {
        [Inject] private readonly CameraDistance _distance;
        [Inject] private readonly CameraConfig _config;
        [Inject] private readonly ICameraBoom _cameraBoom;

        public void AddDistance(float zoomValue)
        {
            _distance.AddDistance(zoomValue, _config.DistanceChangeFactor, _config.MinDistance, _config.MaxDistance);
        }

        public void Tick(float dt)
        {
            _distance.ApplySmoothing(dt, _config.DistanceSmoothing);
            _cameraBoom.SetDistance(_distance.Current);
        }
    }
}