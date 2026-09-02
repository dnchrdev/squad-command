using Feature.Command.Application.CommandStates;
using Feature.Command.Application.CommandStates.Interfaces;
using Zenject;

namespace Feature.Command.Application
{
    public sealed class CommandStateMachine
    {
        [Inject] private readonly DiContainer _container;

        public ICommandState Current { get; private set; }

        private SelectCommandState _select;
        private MoveCommandState _move;
        
        //todo
        //private AttackCursorMode _attack;

        [Inject]
        private void Construct()
        {
            _select = _container.Instantiate<SelectCommandState>();
            _move = _container.Instantiate<MoveCommandState>();
            
            //todo
            //_attack = _container.Instantiate<AttackCursorMode>();
        }

        public void SetSelectState() => Switch(_select);
        public void SetMoveState() => Switch(_move);
        
        //todo
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