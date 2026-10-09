namespace Features.Command.UseCases
{
    public sealed class CommandModeUseCase
    {
        private readonly SelectCommandUseCase _select;
        private readonly MoveCommandUseCase _move;

        public ICommandState Current { get; private set; }

        public CommandModeUseCase(SelectCommandUseCase select, MoveCommandUseCase move)
        {
            _select = select;
            _move = move;
        }

        public void SetSelectMode() => Switch(_select);
        public void SetMoveMode() => Switch(_move);

        private void Switch(ICommandState next)
        {
            if (Current == next) return;
            Current?.OnExit();
            Current = next;
            Current?.OnEnter();
        }
    }
}