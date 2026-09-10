using Feature.GameplayECS.Spawning.UnitFactory;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.View.Systems
{
    public class CleanupViewSystem : ICleanupSystem
    {
        private readonly IUnitViewFactory _unitViewFactory;
        public World World { get; set; }
        private Filter _toDestroyFilter;
        private Filter _allViewsFilter;
        private Stash<View> _viewStash;

        public CleanupViewSystem(IUnitViewFactory unitViewFactory)
        {
            _unitViewFactory = unitViewFactory;
        }

        public void OnAwake()
        {
            _toDestroyFilter = World.Filter.With<DestroySelfRequest>().With<View>().Build();
            _allViewsFilter = World.Filter.With<View>().Build();
            _viewStash = World.GetStash<View>();
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
            if (view.Value == null) return;

            view.Value.Unbind();
            _unitViewFactory.Release(view.Value);
        }
    }
}