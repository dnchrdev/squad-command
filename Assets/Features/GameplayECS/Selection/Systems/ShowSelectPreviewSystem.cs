using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Selection.Components;
using Features.GameplayECS.Selection.Outputs;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Selection.Systems
{
    public class ShowSelectPreviewSystem: ISystem
    {
        public World World { get; set; }
        
        private Filter _selectPreviewFilter;
        private Filter _selectedFilter;
        
        private Stash<SelectedViewShowedTag> _selectedShowedStash;
        private Stash<SelectViewComponent> _selectViewStash;
        
        public void OnAwake()
        {
            _selectPreviewFilter = World.Filter
                .With<UnitTag>()
                .With<SelectPreviewTag>()
                .With<SelectViewComponent>()
                .Without<SelectedViewShowedTag>()
                .Without<UnselectPreviewTag>()
                .Build();
            
            _selectedFilter = World.Filter
                .With<UnitTag>()
                .With<SelectedTag>()
                .With<SelectViewComponent>()
                .Without<SelectedViewShowedTag>()
                .Without<UnselectPreviewTag>()
                .Build();
            
            _selectedShowedStash = World.GetStash<SelectedViewShowedTag>();
            _selectViewStash = World.GetStash<SelectViewComponent>();
        }
        
        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _selectPreviewFilter)
            {
                ref var selectView = ref _selectViewStash.Get(entity);
                selectView.Value.Show();
                
                _selectedShowedStash.Set(entity);
            }
            
            foreach (var entity in _selectedFilter)
            {
                ref var selectView = ref _selectViewStash.Get(entity);
                selectView.Value.Show();
                
                _selectedShowedStash.Set(entity);
            }
            
        }
        
        public void Dispose()
        {
        
        }
    }
}