using Feature.CameraFeature.Adapter;
using Feature.CameraFeature.Infrastructure;
using Feature.CameraFeature.Infrastructure.CameraTransfromControllers;
using UnityEngine;
using Zenject;

namespace Feature.CameraFeature.Installer
{
    public class CameraInstaller : MonoInstaller
    {
        [SerializeField] private ControlledCamera _controlledCamera;
        [SerializeField] private CameraRefs _refs;
        [SerializeField] private CameraConfig _config;

        public override void InstallBindings()
        {
            //Application
            Container.BindInterfacesTo<CameraInputController>().AsSingle().NonLazy();
            
            //Adapter   
            Container.BindInterfacesTo<CameraPosition>().AsSingle();
            Container.BindInterfacesTo<CameraRotation>().AsSingle();
            Container.BindInterfacesTo<CameraBoom>().AsSingle();
            
            //infrastructure
            Container.Bind<IReadOnlyCamera>().To<ControlledCamera>().FromInstance(_controlledCamera).AsSingle();
            Container.Bind<CameraRefs>().FromInstance(_refs).AsSingle();
            Container.Bind<CameraConfig>().FromInstance(_config).AsSingle();
        }
    }   
}