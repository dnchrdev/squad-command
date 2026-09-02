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
        [Inject] private readonly IReadOnlyCommandInput _input;
        [Inject] private readonly CommandStateMachine _commandStateMachine;

        private readonly CompositeDisposable _disposables = new();
        private Vector2 _currentPointer;

        public void Initialize()
        {
            KeyboardSubscribe();
            ViewSubscribe();
        }

        private void KeyboardSubscribe()
        {
            _input.PointerPosition
                .Subscribe(pos =>
                {
                    _currentPointer = pos;
                    _commandStateMachine.Current.OnPointerMove(pos);
                })
                .AddTo(_disposables);

            _input.ClickDown
                .Subscribe(_ => _commandStateMachine.Current.OnPointerDown(_currentPointer))
                .AddTo(_disposables);

            _input.ClickUp
                .Subscribe(_ => _commandStateMachine.Current.OnPointerUp(_currentPointer))
                .AddTo(_disposables);

            _input.ShiftDown
                .Subscribe(_ => _commandStateMachine.Current.SetShiftHold(true))
                .AddTo(_disposables);

            _input.ShiftUp
                .Subscribe(_ => _commandStateMachine.Current.SetShiftHold(false))
                .AddTo(_disposables);

            _input.CtrlDown
                .Subscribe(_ => _commandStateMachine.Current.SetCtrlHold(true))
                .AddTo(_disposables);

            _input.CtrlUp
                .Subscribe(_ => _commandStateMachine.Current.SetCtrlHold(false))
                .AddTo(_disposables);

            _input.Select.Subscribe(_ => _commandStateMachine.SetSelectState()).AddTo(_disposables);
            _input.Move.Subscribe(_ => _commandStateMachine.SetMoveState()).AddTo(_disposables);
        }

        private void ViewSubscribe()
        {
            _commandView.GetClick(CommandType.Select).Subscribe(_ => _commandStateMachine.SetSelectState())
                .AddTo(_disposables);
            _commandView.GetClick(CommandType.Move).Subscribe(_ => _commandStateMachine.SetMoveState()).AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();
    }
}