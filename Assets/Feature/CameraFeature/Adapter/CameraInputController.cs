using Feature.CameraFeature.Adapter.Interfaces;
using Feature.Input.Adapter.Interfaces;
using R3;
using UnityEngine;
using Zenject;

namespace Feature.CameraFeature.Adapter
{
    public class CameraInputController : IInitializable, ITickable
    {
        [Inject] private readonly ICameraPositionUpdate _positionUpdate;
        [Inject] private readonly ICameraRotationUpdate _rotationUpdate;
        [Inject] private readonly ICameraBoomUpdate _boomUpdate;

        [Inject] private readonly IReadOnlyCameraInput _cameraInput;

        public void Initialize()
        {
            _cameraInput.Zoom.Subscribe(value => _boomUpdate.AddDistance(value));
        }

        public void Tick()
        {
            Vector2 moveDelta;
            var isDragging = false;

            if (_cameraInput.IsDragging.CurrentValue)
            {
                moveDelta = _cameraInput.PointerDelta.CurrentValue;
                isDragging = true;
            }
            else
            {
                moveDelta = Vector2.zero;
            }

            Vector2 rotateDelta;

            if (_cameraInput.IsRotating.CurrentValue && isDragging == false)
            {
                rotateDelta = _cameraInput.PointerDelta.CurrentValue;
            }
            else
            {
                rotateDelta = Vector2.zero;
            }

            _positionUpdate.Tick(Time.deltaTime, moveDelta);
            _rotationUpdate.Tick(Time.deltaTime, rotateDelta);
            _boomUpdate.Tick(Time.deltaTime);
        }
    }
}