using Feature.CameraFeature.Adapter.Interfaces;
using Feature.GameLifecycle.Application.Interfaces;
using Feature.Shared.Math;
using UnityEngine;
using Zenject;

namespace Feature.CameraFeature.Infrastructure.CameraTransfromControllers
{
    public class CameraPosition : ICameraPositionUpdate, ICameraPositionReset
    {
        [Inject] private readonly CameraRefs _refs;
        [Inject] private readonly CameraConfig _config;

        private Vector3 _targetPosition;

        public void SetPosition(Vector3 targetPosition)
        {
            _refs.PositionRoot.transform.position = targetPosition;
        }

        public void Tick(float dt, Vector2 delta)
        {
            var forward = Vector3.ProjectOnPlane(_refs.RotationRoot.forward, Vector3.up).normalized;
            var right = Vector3.ProjectOnPlane(_refs.RotationRoot.right, Vector3.up).normalized;

            var additionalDelta = forward * -delta.y + right * -delta.x;

            _targetPosition += additionalDelta * (_config.MoveChangeFactor * dt);
            
            var currentPosition = _refs.PositionRoot.transform.position;
            _refs.PositionRoot.transform.position =
                MathExtensions.TranslationWithSmoothing(currentPosition, _targetPosition, dt, _config.MoveSmoothing);
        }
    }
}