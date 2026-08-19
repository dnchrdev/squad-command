using Feature.Input.Infrastructure;
using Zenject;

namespace Feature.Input.Installer
{
    public class InputInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            //Adapter
            Container.BindInterfacesTo<DesktopInput>().AsSingle().NonLazy();
        }
    }
}