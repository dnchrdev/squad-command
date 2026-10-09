using System;
using Features.Camera.Entities;
using Features.Camera.UseCases;
using Features.Shared.Input;
using R3;
using UnityEngine;
using Zenject;

namespace Features.Camera.InterfaceAdapters.Controllers
{
    public sealed class CameraInputController : IInitializable, ITickable, IDisposable
    {
        private readonly CameraPositionUseCase _positionUseCase;
        private readonly CameraRotationUseCase _rotationUseCase;
        private readonly CameraBoomUseCase _boomUseCase;
        private readonly IReadOnlyCameraInput _cameraInput;
        private IDisposable _zoomSubscription;

        public CameraInputController(
            CameraPositionUseCase positionUseCase,
            CameraRotationUseCase rotationUseCase,
            CameraBoomUseCase boomUseCase,
            IReadOnlyCameraInput cameraInput)
        {
            _positionUseCase = positionUseCase;
            _rotationUseCase = rotationUseCase;
            _boomUseCase = boomUseCase;
            _cameraInput = cameraInput;
        }

        public void Initialize()
        {
            _zoomSubscription = _cameraInput.Zoom.Subscribe(value => _boomUseCase.AddDistance(value));
        }

        public void Tick()
        {
            var input = CameraInputResolver.Resolve(
                _cameraInput.IsDragging.CurrentValue,
                _cameraInput.IsRotating.CurrentValue,
                _cameraInput.PointerDelta.CurrentValue);

            var dt = Time.deltaTime;

            _rotationUseCase.Tick(dt, input.RotateDelta);
            _positionUseCase.Tick(dt, input.MoveDelta);
            _boomUseCase.Tick(dt);
        }

        public void Dispose() => _zoomSubscription?.Dispose();
    }
}