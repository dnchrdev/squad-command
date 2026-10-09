using Features.GameLifecycle.InterfaceAdapters.Controllers;
using Features.GameLifecycle.UseCases;
using Zenject;

namespace Features.GameLifecycle.Installer
{
    public class GameLifecycleInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GameStartUseCase>().AsSingle();
            Container.BindInterfacesTo<GameStartController>().AsSingle().NonLazy();
        }
    }
}