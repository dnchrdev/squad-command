using Feature.Command.Application.CursorModes;
using Feature.Command.Application.CursorModes.Interfaces;
using Zenject;

namespace Feature.Command.Application
{
    public sealed class CommandState
    {
        [Inject] private readonly DiContainer _container;

        public ICursorMode Current { get; private set; }

        private SelectCursorMode _select;
        private MoveCursorMode _move;
        //private AttackCursorMode _attack;

        [Inject]
        private void Construct()
        {
            _select = _container.Instantiate<SelectCursorMode>();
            _move = _container.Instantiate<MoveCursorMode>();
            //_attack = _container.Instantiate<AttackCursorMode>();
            Current = _select;
        }

        public void SetSelect() => Switch(_select);
        public void SetMove() => Switch(_move);
        //public void SetAttack() => Switch(_attack);

        private void Switch(ICursorMode next)
        {
            if (Current == next) return;
            Current.OnExit();
            Current = next;
            Current.OnEnter();
        }
    }
}