using Feature.GameplayECS.Spawning.UnitFactory;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.View.Systems
{
    public class CleanupViewSystem : ICleanupSystem
    {
        private readonly IUnitViewFactory _unitViewFactory;
        public World World { get; set; }

        private Filter _toDestroyFilter;
        private Filter _allViews;

        private Stash<View> _viewStash;

        public CleanupViewSystem (IUnitViewFactory unitViewFactory)
        {
            _unitViewFactory = unitViewFactory;
        }
        
        public void OnAwake()
        {
            _toDestroyFilter = World.Filter
                .With<DestroySelfRequest>()
                .With<View>()
                .Build();
            
            _allViews = World.Filter
                .With<View>()
                .Build();

            _viewStash = World.GetStash<View>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _toDestroyFilter)
            {
                DestriyView(entity);
            }
        }
        
        public void Dispose()
        {
            foreach (var entity in _allViews)
            {
                DestriyView(entity);
            }
        }

        private void DestriyView(Entity entity)
        {
            ref View view = ref _viewStash.Get(entity);

            if (view.Value == null)
                return;
            
            view.Value.Unbind();
            _unitViewFactory.Release(view.Value);
        }

    }
}