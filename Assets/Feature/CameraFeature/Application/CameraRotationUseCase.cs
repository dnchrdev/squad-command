using Feature.CameraFeature.Application.Interfaces;
using Feature.CameraFeature.Domain;
using Feature.CameraFeature.Infrastructure;
using Zenject;
using Vector2 = UnityEngine.Vector2;


namespace Feature.CameraFeature.Application
{
    public class CameraRotationUseCase
    {
        [Inject] private readonly CameraOrientation _orientation;
        [Inject] private readonly CameraConfig _config;
        [Inject] private readonly ICameraRotation _cameraRotation;

        public void Tick(float dt, Vector2 rotateDelta)
        {
            _orientation.SetTarget(rotateDelta, _config.RotationChangeFactor, _config.MinPitch, _config.MaxPitch);
            _orientation.ApplySmoothing(dt, _config.RotationSmoothing);
            _cameraRotation.SetRotation(_orientation.Current);
        }
    }
}