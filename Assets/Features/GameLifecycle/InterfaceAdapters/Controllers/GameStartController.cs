using Features.GameLifecycle.UseCases;
using Zenject;

namespace Features.GameLifecycle.InterfaceAdapters.Controllers
{
    public sealed class GameStartController : IInitializable
    {
        private readonly GameStartUseCase _gameStart;

        public GameStartController(GameStartUseCase gameStart)
        {
            _gameStart = gameStart;
        }

        public void Initialize() => _gameStart.Execute();
    }
}