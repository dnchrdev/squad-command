using Features.GameplayECS.Common.View;
using Scellecs.Morpeh;

namespace Features.GameplayECS.UnitView.Components
{
    public struct UnitViewComponent : IComponent { public MonoEntity Value; }
    public struct AssetPath : IComponent { public string Value; }
    public struct SpawningTag : IComponent { }
}
