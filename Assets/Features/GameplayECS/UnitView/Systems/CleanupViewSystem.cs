using Features.GameplayECS.UnitView.Components;
using Features.GameplayECS.UnitView.Systems.Ports;
using Features.GameplayECS.UnitView.Requests;
using Scellecs.Morpeh;

namespace Features.GameplayECS.UnitView.Systems
{
    public class CleanupViewSystem : ICleanupSystem
    {
        private readonly IUnitViewFactory _unitViewFactory;
        public World World { get; set; }
        private Filter _toDestroyFilter;
        private Filter _allViewsFilter;
        private Stash<UnitViewComponent> _viewStash;

        public CleanupViewSystem(IUnitViewFactory unitViewFactory)
        {
            _unitViewFactory = unitViewFactory;
        }

        public void OnAwake()
        {
            _toDestroyFilter = World.Filter.With<DestroySelfRequest>().With<UnitViewComponent>().Build();
            _allViewsFilter = World.Filter.With<UnitViewComponent>().Build();
            _viewStash = World.GetStash<UnitViewComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _toDestroyFilter)
                DestroyView(entity);
        }

        public void Dispose()
        {
            foreach (var entity in _allViewsFilter)
                DestroyView(entity);
        }

        private void DestroyView(Entity entity)
        {
            ref var view = ref _viewStash.Get(entity);
            view.Value.Unbind();
            _unitViewFactory.Release(view.Value);
        }
    }
}