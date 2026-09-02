using Feature.CameraFeature.Application.Interfaces;
using Feature.CameraFeature.Domain;
using Feature.CameraFeature.Infrastructure;
using UnityEngine;
using Zenject;

namespace Feature.CameraFeature.Application
{
    public class CameraPositionUseCase
    {
        [Inject] private readonly CameraPosition _position;
        [Inject] private readonly CameraConfig _config;
        [Inject] private readonly ICameraPosition _cameraPosition;
        [Inject] private readonly IReadOnlyCameraOrientation _orientationRead;

        public void Tick(float dt, Vector2 moveDelta)
        {
            _position.SetTarget(moveDelta, _orientationRead.Forward, _orientationRead.Right, _config.MoveChangeFactor);
            _position.ApplySmoothing(dt, _config.MoveSmoothing);
            _cameraPosition.SetPosition(_position.Current);
        }
    }
}