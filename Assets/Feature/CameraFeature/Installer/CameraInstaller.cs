using Feature.CameraFeature.Adapter;
using Feature.CameraFeature.Application;
using Feature.CameraFeature.Application.Interfaces;
using Feature.CameraFeature.Domain;
using Feature.CameraFeature.Infrastructure;
using Feature.CameraFeature.Infrastructure.Interfaces;
using Feature.Shared.DevTools;
using UnityEngine;
using Zenject;
using CameraPosition = Feature.CameraFeature.Domain.CameraPosition;

namespace Feature.CameraFeature.Installer
{
    public class CameraInstaller : MonoInstaller
    {
        [SerializeField] private ControlledCamera _controlledCamera;
        [SerializeField] private CameraRefs _refs;
        [SerializeField] private CameraConfig _config;

        private void OnValidate()
        {
            InspectorRefValidator.CheckAssigned(_controlledCamera, nameof(_controlledCamera), this);
            InspectorRefValidator.CheckAssigned(_refs, nameof(_refs), this);
            InspectorRefValidator.CheckAssigned(_config, nameof(_config), this);
        }

        public override void InstallBindings()
        {
            // Domain
            Container.Bind<CameraOrientation>().AsSingle();
            Container.Bind<CameraPosition>().AsSingle();
            Container.Bind<CameraDistance>().AsSingle();

            // Application
            Container.Bind<CameraRotationUseCase>().AsSingle();
            Container.Bind<CameraPositionUseCase>().AsSingle();
            Container.Bind<CameraBoomUseCase>().AsSingle();
            Container.Bind<CameraInputResolverUseCase>().AsSingle();
            Container.Bind<CameraBootstrap>().AsSingle();

            // Adapter
            Container.BindInterfacesTo<CameraInputController>().AsSingle().NonLazy();
            Container.Bind<ICameraPosition>().To<Adapter.CameraPosition>().AsSingle();
            Container.BindInterfacesTo<CameraRotation>().AsSingle();
            Container.Bind<ICameraBoom>().To<CameraBoom>().AsSingle();

            // Infrastructure
            Container.Bind<IReadOnlyCamera>().To<ControlledCamera>().FromInstance(_controlledCamera).AsSingle();
            Container.Bind<CameraRefs>().FromInstance(_refs).AsSingle();
            Container.Bind<CameraConfig>().FromInstance(_config).AsSingle();
        }
    }
}