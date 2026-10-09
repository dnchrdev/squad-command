using System;
using Features.Command.Entities;
using Features.Command.InterfaceAdapters.Ports;
using Features.Command.UseCases;
using Features.Shared.Input;
using R3;
using Vector2 = UnityEngine.Vector2;

namespace Features.Command.InterfaceAdapters.Controllers
{
    public sealed class CommandInputController : IDisposable
    {
        private readonly ICommandView _commandView;
        private readonly IReadOnlyCommandInput _commandInput;
        private readonly IReadOnlyCameraInput _cameraInput;
        private readonly CommandModeUseCase _mode;
        private readonly CompositeDisposable _disposables = new();
        private Vector2 _currentPointer;

        public CommandInputController(
            ICommandView commandView,
            IReadOnlyCommandInput commandInput,
            IReadOnlyCameraInput cameraInput,
            CommandModeUseCase mode)
        {
            _commandView = commandView;
            _commandInput = commandInput;
            _cameraInput = cameraInput;
            _mode = mode;
        }

        public void Initialize()
        {
            _commandInput.PointerPosition.Subscribe(pos =>
            {
                _currentPointer = pos;
                _mode.Current.OnPointerMove(pos);
            }).AddTo(_disposables);

            _commandInput.ClickDown.Subscribe(_ => _mode.Current.OnPointerDown(_currentPointer)).AddTo(_disposables);
            _commandInput.ClickUp.Subscribe(_ => _mode.Current.OnPointerUp(_currentPointer)).AddTo(_disposables);
            _commandInput.ShiftDown.Subscribe(_ => _mode.Current.SetShiftHold(true)).AddTo(_disposables);
            _commandInput.ShiftUp.Subscribe(_ => _mode.Current.SetShiftHold(false)).AddTo(_disposables);
            _commandInput.CtrlDown.Subscribe(_ => _mode.Current.SetCtrlHold(true)).AddTo(_disposables);
            _commandInput.CtrlUp.Subscribe(_ => _mode.Current.SetCtrlHold(false)).AddTo(_disposables);
            _commandInput.Select.Subscribe(_ => _mode.SetSelectMode()).AddTo(_disposables);
            _commandInput.Move.Subscribe(_ => _mode.SetMoveMode()).AddTo(_disposables);

            _cameraInput.DragStarted.Subscribe(_ => _mode.Current.Canceled()).AddTo(_disposables);
            _cameraInput.RotationStarted.Subscribe(_ => _mode.Current.Canceled()).AddTo(_disposables);

            _commandView.GetClick(CommandType.SELECT).Subscribe(_ => _mode.SetSelectMode()).AddTo(_disposables);
            _commandView.GetClick(CommandType.MOVE).Subscribe(_ => _mode.SetMoveMode()).AddTo(_disposables);
        }

        public void Dispose() => _disposables.Dispose();
    }
}