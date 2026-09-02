using Feature.Command.Application.CommandModes;
using Feature.Command.Application.CommandModes.Interfaces;
using Zenject;

namespace Feature.Command.Application
{
    public sealed class CommandStateMachine
    {
        [Inject] private readonly DiContainer _container;

        public ICommandState Current { get; private set; }

        private SelectCommandState _select;
        private MoveCommandState _move;
        //private AttackCursorMode _attack;

        [Inject]
        private void Construct()
        {
            _select = _container.Instantiate<SelectCommandState>();
            _move = _container.Instantiate<MoveCommandState>();
            //_attack = _container.Instantiate<AttackCursorMode>();
        }

        public void SetSelect() => Switch(_select);
        public void SetMove() => Switch(_move);
        //public void SetAttack() => Switch(_attack);

        private void Switch(ICommandState next)
        {
            if (Current == next) return;
            Current?.OnExit();
            Current = next;
            Current?.OnEnter();
        }
    }
}