using Feature.GameplayECS.Facade;
using Feature.GameplayECS.Facade.Interfaces;
using Feature.GameplayECS.MoveCommand.MovePreviewFactory;
using Feature.GameplayECS.Spawning.UnitFactory;
using Scellecs.Morpeh;
using Zenject;

namespace Feature.GameplayECS.Installer
{
    public class GameplayEcsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //Application
            Container.Bind<World>().FromInstance(World.Default).AsSingle();
            Container.BindInterfacesAndSelfTo<GameRunner.GameRunner>().AsSingle();
            Container.Bind<IUnitQueryFacade>().To<UnitQueryFacade>().AsSingle();
            Container.Bind<ISelectCommandFacade>().To<SelectCommandFacade>().AsSingle();
            Container.Bind<IMoveCommandFacade>().To<MoveCommandFacade>().AsSingle();

            //Infrastructure
            Container.Bind<IUnitViewFactory>().To<AddressableUnitViewFactory>().AsSingle();
            Container.Bind<IMovePreviewFactory>().To<MovePreviewFactory>().AsSingle();
        }
    }
}