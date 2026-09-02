using Feature.CameraFeature.Adapter.Interfaces;
using Feature.CameraFeature.Domain;
using Feature.GameLifecycle.Application.Interfaces;
using Feature.Shared.Math;
using UnityEngine;
using Zenject;

namespace Feature.CameraFeature.Infrastructure.CameraTransfromControllers
{
    public class CameraRotation: ICameraRotationUpdate, ICameraRotationReset
    {
        [Inject] private readonly CameraRefs _refs;
        [Inject] private readonly CameraConfig _config;
        
        public void SetRotation(Quaternion targetRotation)
        {
            var clampedRotation = CameraRotationMath.ClampPitch(targetRotation, _config.MinPitch, _config.MaxPitch);
            _refs.RotationRoot.transform.rotation = clampedRotation;
        }
        
        public void Tick(float dt, Vector2 delta)
        {
            var currentYaw = _refs.RotationRoot.transform.eulerAngles.y;
            var currentPitch = _refs.RotationRoot.transform.eulerAngles.x;
            
            var targetYaw = currentYaw + delta.x * _config.RotationChangeFactor;
            var targetPitch = currentPitch + -delta.y * _config.RotationChangeFactor;
            
            var currentRotation = _refs.RotationRoot.transform.rotation;
            var targetRotation = Quaternion.Euler(targetPitch, targetYaw, 0f);
            
            var clampedTargetRotation =  CameraRotationMath.ClampPitch(targetRotation, _config.MinPitch, _config.MaxPitch);
            
            _refs.RotationRoot.transform.rotation = MathExtensions.TranslationWithSmoothing(currentRotation, clampedTargetRotation, dt, _config.RotationSmoothing);
        }
        
    }
}