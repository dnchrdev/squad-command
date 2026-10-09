using Features.GameplayECS.UnitView.Systems.Ports;
using Features.GameplayECS.UnitView.Systems;
using Scellecs.Morpeh;

namespace Features.GameplayECS.UnitView
{
    public static class UnitViewFeature
    {
        public static void Add(SystemsGroup group, IUnitViewFactory unitViewFactory)
        {
            group.AddSystem(new CreateViewSystem(unitViewFactory));
            group.AddSystem(new CleanupViewSystem(unitViewFactory));
        }
    }
}