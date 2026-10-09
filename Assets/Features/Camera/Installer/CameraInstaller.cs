using Features.Camera.Entities;
using Features.Camera.Infrastructure;
using Features.Camera.InterfaceAdapters.Controllers;
using Features.Camera.InterfaceAdapters.Gateways;
using Features.Camera.UseCases;
using Features.Camera.UseCases.Ports;
using Features.Shared.DevTools;
using Features.Shared.Lifecycle;
using Features.Shared.Projection;
using UnityEngine;
using Zenject;

namespace Features.Camera.Installer
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
            InstallEntities();
            InstallUseCases();
            InstallInterfaceAdapters();
            InstallInfrastructure();
        }

        private void InstallEntities()
        {
            Container.Bind<CameraSettings>().FromInstance(_config.ToSettings()).AsSingle();
            Container.Bind<CameraOrientation>().AsSingle();
            Container.Bind<CameraPosition>().AsSingle();
            Container.Bind<CameraDistance>().AsSingle();
        }

        private void InstallUseCases()
        {
            Container.Bind<CameraRotationUseCase>().AsSingle();
            Container.Bind<CameraPositionUseCase>().AsSingle();
            Container.Bind<CameraBoomUseCase>().AsSingle();
            Container.Bind<ICameraStart>().To<CameraStartUseCase>().AsSingle();
        }

        private void InstallInterfaceAdapters()
        {
            Container.BindInterfacesTo<CameraInputController>().AsSingle().NonLazy();
            Container.Bind<ICameraPosition>().To<CameraPositionGateway>().AsSingle();
            Container.BindInterfacesTo<CameraRotationGateway>().AsSingle();
            Container.Bind<ICameraBoom>().To<CameraBoomGateway>().AsSingle();
            Container.Bind<IScreenProjection>().To<ScreenProjectionGateway>().AsSingle();
        }

        private void InstallInfrastructure()
        {
            Container.Bind<ControlledCamera>().FromInstance(_controlledCamera).AsSingle();
            Container.Bind<CameraRefs>().FromInstance(_refs).AsSingle();
        }
    }
}