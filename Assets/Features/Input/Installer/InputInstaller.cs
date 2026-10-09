using Features.Input.InterfaceAdapters.Gateways;
using Zenject;

namespace Features.Input.Installer
{
    public class InputInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            //Adapter
            Container.BindInterfacesTo<DesktopInputGateway>().AsSingle().NonLazy();
        }
    }
}