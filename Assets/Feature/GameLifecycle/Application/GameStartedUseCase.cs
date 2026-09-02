using Feature.CameraFeature.Application;
using Feature.Command.Application;
using Zenject;

namespace Feature.GameLifecycle.Application
{
    public class GameStartedUseCase : IInitializable
    {
        [Inject] private readonly CameraBootstrap _cameraBootstrap;
        [Inject] private readonly CommandBootstrap _commandBootstrap;

        public void Initialize()
        {
            _cameraBootstrap.GameStarted();
            _commandBootstrap.GameStarted();
        }
    }
}