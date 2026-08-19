using Feature.Command.Adapter;
using Feature.Command.Adapter.Interfaces;
using Feature.Command.Application;
using Feature.Command.Domain;
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
        
        public override void InstallBindings()
        {
            //Domain
            Container.BindInterfacesAndSelfTo<CommandState>().AsSingle();

            // Application
            Container.Bind<SelectionUseCase>().AsSingle();
            Container.Bind<MoveSquadUseCase>().AsSingle();
            Container.BindInterfacesAndSelfTo<CommandPresenter>().AsSingle();

            //Infrastructure
            Container.BindInterfacesTo<SelectRectView>().FromInstance(_selectRectView).AsSingle();
            Container.Bind<ISelectionQueryService>()
                .To<SelectionQueryService>()
                .AsSingle();

            Container.Bind<IGroundQueryService>()
                .To<GroundQueryService>()
                .AsSingle();
            Container.Bind<CommandConfig>().FromInstance(_config).AsSingle();
        }
    }
}