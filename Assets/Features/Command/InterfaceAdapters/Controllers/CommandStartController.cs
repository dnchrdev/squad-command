using Features.Command.InterfaceAdapters.Ports;
using Features.Command.UseCases;
using Features.Shared.Lifecycle;

namespace Features.Command.InterfaceAdapters.Controllers
{
    public sealed class CommandStartController : ICommandStart
    {
        private readonly IMoveVisualView _moveVisualView;
        private readonly ISelectRectView _selectRectView;
        private readonly ICommandView _commandView;
        private readonly CommandInputController _inputController;
        private readonly CommandModeUseCase _mode;

        public CommandStartController(
            IMoveVisualView moveVisualView,
            ISelectRectView selectRectView,
            ICommandView commandView,
            CommandInputController inputController,
            CommandModeUseCase mode)
        {
            _moveVisualView = moveVisualView;
            _selectRectView = selectRectView;
            _commandView = commandView;
            _inputController = inputController;
            _mode = mode;
        }

        public void Start()
        {
            _moveVisualView.Initialize();
            _selectRectView.Initialize();
            _commandView.Initialize();
            _inputController.Initialize();
            _mode.SetSelectMode();
        }
    }
}