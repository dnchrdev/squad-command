using Feature.Input.Adapter.Interfaces;
using R3;
using UnityEngine;
using Zenject;

namespace Feature.CameraFeature.Application
{
    public class CameraInputController : IInitializable, ITickable
    {
        [Inject] private readonly CameraPositionUseCase _positionUseCase;
        [Inject] private readonly CameraRotationUseCase _rotationUseCase;
        [Inject] private readonly CameraBoomUseCase _boomUseCase;
        [Inject] private readonly CameraInputResolverUseCase _inputResolver;
        [Inject] private readonly IReadOnlyCameraInput _cameraInput;

        public void Initialize()
        {
            _cameraInput.Zoom.Subscribe(value => _boomUseCase.AddDistance(value));
        }

        public void Tick()
        {
            var input = _inputResolver.Resolve(
                _cameraInput.IsDragging.CurrentValue,
                _cameraInput.IsRotating.CurrentValue,
                _cameraInput.PointerDelta.CurrentValue);

            var dt = Time.deltaTime;

            _rotationUseCase.Tick(dt, input.RotateDelta);
            _positionUseCase.Tick(dt, input.MoveDelta);
            _boomUseCase.Tick(dt);
        }
    }
}