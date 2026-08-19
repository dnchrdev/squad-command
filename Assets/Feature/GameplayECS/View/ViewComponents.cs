using Scellecs.Morpeh;

namespace Feature.GameplayECS.View
{
    public struct AssetPath: IComponent { public string Value; }
    public struct View : IComponent { public MonoEntity Value; }
    public struct Spawning : IComponent { }
}