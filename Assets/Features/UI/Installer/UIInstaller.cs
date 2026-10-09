using Features.Shared.UI;
using Features.UI.InterfaceAdapters.Gateways;
using Zenject;

namespace Features.UI.Installer
{
    public class UIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IValueAnimator>().To<ValueAnimatorGateway>().AsSingle();
        }
    }
}