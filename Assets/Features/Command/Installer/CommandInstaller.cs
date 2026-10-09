using Features.Command.Entities;
using Features.Command.Infrastructure;
using Features.Command.InterfaceAdapters.Controllers;
using Features.Command.InterfaceAdapters.Gateways;
using Features.Command.InterfaceAdapters.Ports;
using Features.Command.InterfaceAdapters.Presenters;
using Features.Command.UseCases;
using Features.Command.UseCases.Ports;
using Features.Shared.DevTools;
using Features.Shared.Lifecycle;
using Features.Shared.Projection;
using UnityEngine;
using Zenject;

namespace Features.Command.Installer
{
    public class CommandInstaller : MonoInstaller
    {
        [SerializeField] private CommandConfig _config;
        [SerializeField] private SelectRectView _selectRectView;
        [SerializeField] private MoveVisualView _moveVisualView;
        [SerializeField] private CommandView _commandView;

        private void OnValidate()
        {
            InspectorRefValidator.CheckAssigned(_config, nameof(_config), this);
            InspectorRefValidator.CheckAssigned(_selectRectView, nameof(_selectRectView), this);
            InspectorRefValidator.CheckAssigned(_moveVisualView, nameof(_moveVisualView), this);
            InspectorRefValidator.CheckAssigned(_commandView, nameof(_commandView), this);
        }

        public override void InstallBindings()
        {
            InstallEntities();
            InstallUseCases();
            InstallInterfaceAdapters();
            InstallInfrastructure();
        }

        private void InstallEntities()
        {
            Container.Bind<CommandSettings>().FromInstance(_config.ToSettings()).AsSingle();
        }

        private void InstallUseCases()
        {
            Container.Bind<SelectCommandUseCase>().AsSingle();
            Container.Bind<MoveCommandUseCase>().AsSingle();
            Container.Bind<CommandModeUseCase>().AsSingle();
        }

        private void InstallInterfaceAdapters()
        {
            Container.BindInterfacesAndSelfTo<CommandInputController>().AsSingle();
            Container.Bind<ICommandStart>().To<CommandStartController>().AsSingle();

            Container.Bind<ISelectionFrameOutput>().To<SelectionFramePresenter>().AsSingle();
            Container.Bind<IMoveDragOutput>().To<MoveDragPresenter>().AsSingle();
            Container.Bind<ICommandModeOutput>().To<CommandButtonsPresenter>().AsSingle();

            Container.Bind<IGroundQuery>()
                .FromMethod(ctx => new GroundQueryGateway(
                    ctx.Container.Resolve<IScreenProjection>(),
                    _config.GroundMask,
                    _config.GroundRaycastDistance))
                .AsSingle();
            Container.Bind<IScreenUnitQuery>().To<ScreenUnitQueryGateway>().AsSingle();
        }

        private void InstallInfrastructure()
        {
            Container.Bind<ICommandView>().FromInstance(_commandView).AsSingle();
            Container.Bind<ISelectRectView>().FromInstance(_selectRectView).AsSingle();
            Container.Bind<IMoveVisualView>().FromInstance(_moveVisualView).AsSingle();
        }
    }
}