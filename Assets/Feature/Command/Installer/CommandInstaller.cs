using Feature.Command.Adapter;
using Feature.Command.Application;
using Feature.Command.Application.Interfaces;
using Feature.Command.Infrastructure;
using Feature.Command.Infrastructure.Configs;
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

        public override void InstallBindings()
        {
            //Domain
            Container.BindInterfacesAndSelfTo<CommandStateMachine>().AsSingle();

            // Application
            Container.Bind<MovePreviewService>().AsSingle();
            Container.Bind<CommandButtonVisual>().AsSingle();
            Container.BindInterfacesAndSelfTo<CommandPresenter>().AsSingle();

            //Interface Adapters
            Container.Bind<ICommandView>().To<CommandView>().FromInstance(_commandView).AsSingle();
            Container.BindInterfacesTo<SelectRectView>().FromInstance(_selectRectView).AsSingle();
            Container.BindInterfacesTo<MoveVisualView>().FromInstance(_moveVisualView).AsSingle();
            Container.Bind<ISelectionQueryService>()
                .To<SelectionQueryService>()
                .AsSingle();

            Container.Bind<IGroundQueryService>()
                .To<GroundQueryService>()
                .AsSingle();
            
            //Infrastructure
            Container.Bind<CommandConfig>().FromInstance(_config).AsSingle();
        }
    }
}