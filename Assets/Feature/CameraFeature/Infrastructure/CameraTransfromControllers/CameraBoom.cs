using Feature.CameraFeature.Adapter.Interfaces;
using Feature.GameLifecycle.Application.Interfaces;
using Feature.Shared.Math;
using UnityEngine;
using Zenject;

namespace Feature.CameraFeature.Infrastructure.CameraTransfromControllers
{
    public class CameraBoom:  ICameraBoomUpdate, ICameraBoomReset
    {
        [Inject] private readonly CameraRefs _refs;
        [Inject] private readonly CameraConfig _config;
        
        private float _targetDistance;
        
        public void SetDistance(float targetDistance)
        {
            _targetDistance = targetDistance;
            _refs.BoomRoot.transform.localPosition = new Vector3(0f, 0f, -targetDistance);
        }
        
        public void AddDistance(float zoomValue)
        {
            _targetDistance += zoomValue * _config.DistanceChangeFactor;
            _targetDistance = Mathf.Clamp(_targetDistance, _config.MinDistance, _config.MaxDistance);
        }
        
        public void Tick(float dt)
        {
            var currentDistance = _refs.BoomRoot.transform.localPosition.z;
            var lerpDistance =  MathExtensions.TranslationWithSmoothing(currentDistance,  -_targetDistance, dt, _config.DistanceSmoothing);
            
            _refs.BoomRoot.transform.localPosition = new Vector3(0f, 0f, lerpDistance);
        }


        
    }
}