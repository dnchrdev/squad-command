using Features.GameplayECS.Facade;
using Features.GameplayECS.MoveCommand.InterfaceAdapters.Gateways;
using Features.GameplayECS.MoveCommand.Ports;
using Features.GameplayECS.Navigation.InterfaceAdapters.Gateways;
using Features.GameplayECS.Navigation.Systems.Ports;
using Features.GameplayECS.Physics.InterfaceAdapters.Gateways;
using Features.GameplayECS.Physics.Systems.Ports;
using Features.GameplayECS.Runner;
using Features.GameplayECS.UnitView.InterfaceAdapters.Gateways;
using Features.GameplayECS.UnitView.Systems.Ports;
using Features.Shared.Units;
using Scellecs.Morpeh;
using Zenject;

namespace Features.GameplayECS.Installer
{
    public class GameplayEcsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<World>().FromInstance(World.Default).AsSingle();
            Container.BindInterfacesAndSelfTo<GameRunner>().AsSingle();

            Container.Bind<IUnitQuery>().To<UnitQueryGateway>().AsSingle();
            Container.Bind<ISelectionCommands>().To<SelectionCommandsGateway>().AsSingle();
            Container.Bind<IMoveCommands>().To<MoveCommandsGateway>().AsSingle();

            Container.Bind<IUnitViewFactory>().To<AddressableUnitViewFactory>().AsSingle();
            Container.Bind<IMoveSlotViewFactory>().To<AddressableMoveSlotViewFactory>().AsSingle();
            Container.Bind<IPathService>().To<NavMeshPathGateway>().AsSingle();
            Container.Bind<IRandomSource>().To<UnityRandomGateway>().AsSingle();
        }
    }
}