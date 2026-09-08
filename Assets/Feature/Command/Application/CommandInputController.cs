using System;
using Feature.Command.Application.Interfaces;
using Feature.Command.Domain.Data;
using Feature.Input.Adapter.Interfaces;
using R3;
using Zenject;
using Vector2 = UnityEngine.Vector2;

namespace Feature.Command.Application
{
    public sealed class CommandInputController : IDisposable
    {
        [Inject] private readonly ICommandView _commandView;
        [Inject] private readonly IReadOnlyCommandInput _comandInput;
        [Inject] private readonly IReadOnlyCameraInput  _cameraInput;
        [Inject] private readonly CommandStateMachine _commandStateMachine;
        private readonly CompositeDisposable _disposables = new();
        private Vector2 _currentPointer;

        public void Initialize()
        {
            _comandInput.PointerPosition.Subscribe(pos =>
            {
                _currentPointer = pos;
                _commandStateMachine.Current.OnPointerMove(pos);
            }).AddTo(_disposables);
            
            _comandInput.ClickDown.Subscribe(_ => _commandStateMachine.Current.OnPointerDown(_currentPointer)).AddTo(_disposables);
            _comandInput.ClickUp.Subscribe(_ => _commandStateMachine.Current.OnPointerUp(_currentPointer)).AddTo(_disposables);
            _comandInput.ShiftDown.Subscribe(_ => _commandStateMachine.Current.SetShiftHold(true)).AddTo(_disposables);
            _comandInput.ShiftUp.Subscribe(_ => _commandStateMachine.Current.SetShiftHold(false)).AddTo(_disposables);
            _comandInput.CtrlDown.Subscribe(_ => _commandStateMachine.Current.SetCtrlHold(true)).AddTo(_disposables);
            _comandInput.CtrlUp.Subscribe(_ => _commandStateMachine.Current.SetCtrlHold(false)).AddTo(_disposables);
            _comandInput.Select.Subscribe(_ => _commandStateMachine.SetSelectState()).AddTo(_disposables);
            _comandInput.Move.Subscribe(_ => _commandStateMachine.SetMoveState()).AddTo(_disposables);
            
            _cameraInput.DragStarted.Subscribe(_ => _commandStateMachine.Current.Canceled()).AddTo(_disposables);
            _cameraInput.RotationStarted.Subscribe(_ => _commandStateMachine.Current.Canceled()).AddTo(_disposables);
            
            _commandView.GetClick(CommandType.Select).Subscribe(_ => _commandStateMachine.SetSelectState()).AddTo(_disposables);
            _commandView.GetClick(CommandType.Move).Subscribe(_ => _commandStateMachine.SetMoveState()).AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();
    }
}