using Feature.CameraFeature.Infrastructure.Interfaces;
using Feature.Command.Adapter;
using Feature.Command.Application;
using Feature.Command.Application.Interfaces;
using Feature.Command.Domain.Services;
using Feature.Command.Infrastructure.Configs;
using Feature.GameplayECS.Facade.Interfaces;
using Feature.Shared.DevTools;
using UnityEngine;
using Zenject;

namespace Feature.Command.Installer
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
            Container.BindInterfacesAndSelfTo<CommandStateMachine>().AsSingle();
            Container.Bind<CommandBootstrap>().AsSingle();
            Container.Bind<CommandButtonsVisual>().AsSingle();
            Container.BindInterfacesAndSelfTo<CommandInputController>().AsSingle();

            Container.Bind<ICommandView>().To<CommandView>().FromInstance(_commandView).AsSingle();
            Container.BindInterfacesTo<SelectRectView>().FromInstance(_selectRectView).AsSingle();
            Container.BindInterfacesTo<MoveVisualView>().FromInstance(_moveVisualView).AsSingle();

            Container.Bind<IGroundQueryService>()
                .To<GroundQueryService>()
                .FromMethod(ctx => new GroundQueryService(
                    ctx.Container.Resolve<IReadOnlyCamera>(),
                    _config.GroundMask))
                .AsSingle();

            Container.Bind<ISelectionQueryService>()
                .To<SelectionQueryService>()
                .FromMethod(ctx => new SelectionQueryService(
                    _config.SingleSelectMaxThreshold,
                    ctx.Container.Resolve<IReadOnlyCamera>(),
                    ctx.Container.Resolve<IUnitQueryFacade>()))
                .AsSingle();
        }
    }
}