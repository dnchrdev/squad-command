using Feature.GameplayECS.CommandProcessing;
using Feature.GameplayECS.Infrastructure.UnitFactory;
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
            Container.Bind<ICommandSquadFacade>().To<CommandSquadFacade>().AsSingle();

            //Infrastructure
            Container.Bind<IUnitViewFactory>().To<AddressableUnitViewFactory>().AsSingle();
        }
    }
}