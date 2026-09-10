using Feature.GameplayECS.Common;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.SelectCommand.Systems
{
    public class ShowSelectPreviewSystem: ISystem
    {
        public World World { get; set; }
        
        private Filter _selectSelfUnitsFilter;
        private Filter _selectedFilter;
        
        private Stash<SelectedViewShowed> _selectedShowedStash;
        private Stash<SelectViewComponent> _selectViewStash;
        
        public void OnAwake()
        {
            _selectSelfUnitsFilter = World.Filter
                .With<UnitTag>()
                .With<SelectSelfRequest>()
                .With<SelectViewComponent>()
                .Without<SelectedViewShowed>()
                .Without<UnselectSelfRequest>()
                .Build();
            
            _selectedFilter = World.Filter
                .With<UnitTag>()
                .With<Selected>()
                .With<SelectViewComponent>()
                .Without<SelectedViewShowed>()
                .Without<UnselectSelfRequest>()
                .Build();
            
            _selectedShowedStash = World.GetStash<SelectedViewShowed>();
            _selectViewStash = World.GetStash<SelectViewComponent>();
        }
        
        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _selectSelfUnitsFilter)
            {
                ref var selectedView = ref _selectViewStash.Get(entity);
                selectedView.Value.Show();
                
                _selectedShowedStash.Set(entity);
            }
            
            foreach (var entity in _selectedFilter)
            {
                ref var selectedView = ref _selectViewStash.Get(entity);
                selectedView.Value.Show();
                
                _selectedShowedStash.Set(entity);
            }
            
        }
        
        public void Dispose()
        {
        
        }
    }
}