using Feature.UI.Application;
using Zenject;

namespace Feature.UI.Installer
{
    public class UIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<UIAnimator>().AsSingle().NonLazy();
        }
    }
}