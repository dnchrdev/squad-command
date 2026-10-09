using Features.Shared.Lifecycle;

namespace Features.GameLifecycle.UseCases
{
    public sealed class GameStartUseCase
    {
        private readonly ICameraStart _cameraStart;
        private readonly ICommandStart _commandStart;

        public GameStartUseCase(ICameraStart cameraStart, ICommandStart commandStart)
        {
            _cameraStart = cameraStart;
            _commandStart = commandStart;
        }

        public void Execute()
        {
            _cameraStart.Start();
            _commandStart.Start();
        }
    }
}