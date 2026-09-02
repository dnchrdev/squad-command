using Zenject;

namespace Feature.Core.Installer
{
    public class CoreInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            //Container.Bind<IObjectPool<T>>().To<ObjectPool<T>>().AsSingle();
        }
    }
}