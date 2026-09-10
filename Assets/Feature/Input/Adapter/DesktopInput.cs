using System;
using System.Collections.Generic;
using Feature.Input.Adapter.Interfaces;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Feature.Input.Adapter
{
    public class DesktopInput : IInitializable, IDisposable, IReadOnlyCameraInput, IReadOnlyCommandInput
    {
        private readonly ReactiveProperty<Vector2> _pointerDelta = new(Vector2.zero);
        private readonly Subject<float> _zoom = new();
        private readonly Subject<Unit> _dragStarted = new();
        private readonly ReactiveProperty<bool> _isDragging = new(false);
        private readonly Subject<Unit> _rotationStarted = new();
        private readonly ReactiveProperty<bool> _isRotating = new(false);
        
        private readonly ReactiveProperty<Vector2> _pointerPosition = new(Vector2.zero);
        private readonly Subject<Unit> _clickDown = new();
        private readonly Subject<Unit> _clickUp = new();
        
        private readonly Subject<Unit> _shiftDown = new();
        private readonly Subject<Unit> _shiftUp = new();
        
        private readonly Subject<Unit> _ctrlDown = new();
        private readonly Subject<Unit> _ctrlUp = new();
        
        private readonly Subject<Unit> _select = new();
        private readonly Subject<Unit> _move = new();
        private readonly Subject<Unit> _attack = new();

        public ReadOnlyReactiveProperty<Vector2> PointerDelta => _pointerDelta;
        public Observable<float> Zoom => _zoom;
        public Observable<Unit> DragStarted => _dragStarted;
        public ReadOnlyReactiveProperty<bool> IsDragging => _isDragging;
        public Observable<Unit> RotationStarted => _rotationStarted;
        public ReadOnlyReactiveProperty<bool> IsRotating => _isRotating;
        
        public ReadOnlyReactiveProperty<Vector2> PointerPosition => _pointerPosition;
        public Observable<Unit> ClickDown => _clickDown;
        public Observable<Unit> ClickUp => _clickUp;
            
        public Observable<Unit> ShiftDown => _shiftDown;
        public Observable<Unit> ShiftUp => _shiftUp;
        
        public Observable<Unit> CtrlDown => _ctrlDown;
        public Observable<Unit> CtrlUp => _ctrlUp;
            
        public Observable<Unit> Select => _select;
        public Observable<Unit> Move => _move;
        public Observable<Unit> Attack => _attack;

        private InputActions _inputActions;
        private readonly List<Action> _unsubscribers = new();

        public void Initialize()
        {
            _inputActions = new InputActions();
            _inputActions.Enable();

            SubscribeInputs();
        }

        private void SubscribeInputs()
        {
            Bind(_inputActions.Camera.Move,
                performed: ctx => _pointerDelta.Value = ctx.ReadValue<Vector2>(),
                canceled: ctx => _pointerDelta.Value = Vector2.zero);

            Bind(_inputActions.Camera.Zoom,
                performed: ctx => _zoom.OnNext(ctx.ReadValue<Vector2>().y));

            Bind(_inputActions.Camera.EnableDragging,
                started: ctx => _dragStarted.OnNext(Unit.Default));
            
            Bind(_inputActions.Camera.EnableDragging,
                started: ctx => _isDragging.Value = true,
                canceled: ctx => _isDragging.Value = false);
            
            Bind(_inputActions.Camera.EnableRotation,
                started: ctx => _rotationStarted.OnNext(Unit.Default));

            Bind(_inputActions.Camera.EnableRotation,
                started: ctx => _isRotating.Value = true,
                canceled: ctx => _isRotating.Value = false);
            
            
            Bind(_inputActions.Command.PointerPosition,
                started: ctx => _pointerPosition.Value = ctx.ReadValue<Vector2>(),
                performed: ctx => _pointerPosition.Value = ctx.ReadValue<Vector2>(),
                canceled: ctx => _pointerPosition.Value = ctx.ReadValue<Vector2>());
            
            Bind(_inputActions.Command.Click,
                started: ctx => _clickDown.OnNext(Unit.Default),
                canceled: ctx => _clickUp.OnNext(Unit.Default));
            
            Bind(_inputActions.Command.Shift,
                started: ctx => _shiftDown.OnNext(Unit.Default),
                canceled: ctx => _shiftUp.OnNext(Unit.Default));

            Bind(_inputActions.Command.Ctrl,
                started: ctx => _ctrlDown.OnNext(Unit.Default),
                canceled: ctx => _ctrlUp.OnNext(Unit.Default));
            
            Bind(_inputActions.Command.Select,
                started: ctx => _select.OnNext(Unit.Default));
            
            Bind(_inputActions.Command.Move,
                started: ctx => _move.OnNext(Unit.Default));
            
            Bind(_inputActions.Command.Attack,
                started: ctx => _attack.OnNext(Unit.Default));
            
        }
    
        private void Bind(
            InputAction action,
            Action<InputAction.CallbackContext> started = null,
            Action<InputAction.CallbackContext> performed = null,
            Action<InputAction.CallbackContext> canceled = null)
        {
            if (started != null)
            {
                action.started += started;
                _unsubscribers.Add(() => action.started -= started);
            }

            if (performed != null)
            {
                action.performed += performed;
                _unsubscribers.Add(() => action.performed -= performed);
            }

            if (canceled != null)
            {
                action.canceled += canceled;
                _unsubscribers.Add(() => action.canceled -= canceled);
            }
        }

        public void Dispose()
        {
            foreach (var unsubscribe in _unsubscribers)
                unsubscribe();
        
            _unsubscribers.Clear();

            _inputActions.Disable();
            _inputActions.Dispose();

            _pointerDelta.Dispose();
            _zoom.Dispose();
            _isDragging.Dispose();
            _isRotating.Dispose();
        }
    }
}