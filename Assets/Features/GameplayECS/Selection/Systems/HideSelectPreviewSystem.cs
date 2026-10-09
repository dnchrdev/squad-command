using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Selection.Components;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Selection.Systems
{
    public class HideSelectPreviewSystem: ISystem
    {
        public World World { get; set; }

        private Filter _allUnselectedFilter;
        private Filter _allUnselectedPreviewFilter;
        
        private Stash<SelectedViewShowedTag> _selectedShowedStash;
        private Stash<SelectViewComponent> _selectViewStash;
        
        public void OnAwake()
        {
            _allUnselectedFilter = World.Filter
                .With<UnitTag>()
                .With<SelectedViewShowedTag>()
                .With<SelectViewComponent>()
                .Without<SelectPreviewTag>()
                .Without<UnselectPreviewTag>()
                .Without<SelectedTag>()
                .Build();
            
            _allUnselectedPreviewFilter = World.Filter
                .With<UnitTag>()
                .With<SelectedViewShowedTag>()
                .With<SelectViewComponent>()
                .With<UnselectPreviewTag>()
                .Without<SelectPreviewTag>()
                .Build();
            
            _selectedShowedStash = World.GetStash<SelectedViewShowedTag>();
            _selectViewStash = World.GetStash<SelectViewComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _allUnselectedFilter)
            {
                ref var selectedView = ref _selectViewStash.Get(entity);
                selectedView.Value.Hide();
                
                _selectedShowedStash.Remove(entity);
            }
            
            foreach (var entity in  _allUnselectedPreviewFilter)
            {
                ref var selectedView = ref _selectViewStash.Get(entity);
                selectedView.Value.Hide();
                
                _selectedShowedStash.Remove(entity);
            }
            
        }

        public void Dispose()
        {
 
        }
    }
}