using Feature.Command.Application.Interfaces;
using Feature.Input.Adapter.Interfaces;
using Zenject;

namespace Feature.Command.Application
{
    public class CommandBootstrap
    {
        [Inject] private readonly IMoveVisualView _moveVisualView;
        [Inject] private readonly ISelectRectView _selectRectView;
        [Inject] private readonly ICommandView _commandView;
        [Inject] private readonly CommandInputController _commandInputController;
        [Inject] private readonly CommandStateMachine _commandStateMachine;

        public void GameStarted()
        {
            _moveVisualView.Initialize();
            _selectRectView.Initialize();
            _commandView.Initialize();
            _commandInputController.Initialize();
            _commandStateMachine.SetSelectState();
        }
    }
}