using Feature.GameLifecycle.Application;
using Zenject;

namespace Feature.GameLifecycle.Installer
{
    public class GameLifecycleInstaller: MonoInstaller
    {
        public override  void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameStartedUseCase>().AsSingle().NonLazy();
        }
    }
}