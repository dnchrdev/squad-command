using Feature.GameplayECS.Common;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.SelectCommand.Systems
{
    public class HideSelectPreviewSystem: ISystem
    {
        public World World { get; set; }

        private Filter _allUnselectedUnitsFilter;
        private Filter _allUnselectedRequestedUnitsFilter;
        
        private Stash<SelectedViewShowed> _selectedShowedStash;
        private Stash<SelectViewComponent> _selectViewStash;
        
        public void OnAwake()
        {
            _allUnselectedUnitsFilter = World.Filter
                .With<UnitTag>()
                .With<SelectedViewShowed>()
                .With<SelectViewComponent>()
                .Without<SelectSelfRequest>()
                .Without<UnselectSelfRequest>()
                .Without<Selected>()
                .Build();
            
            _allUnselectedRequestedUnitsFilter = World.Filter
                .With<UnitTag>()
                .With<SelectedViewShowed>()
                .With<SelectViewComponent>()
                .With<UnselectSelfRequest>()
                .Without<SelectSelfRequest>()
                .Build();
            
            _selectedShowedStash = World.GetStash<SelectedViewShowed>();
            _selectViewStash = World.GetStash<SelectViewComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _allUnselectedUnitsFilter)
            {
                ref var selectedView = ref _selectViewStash.Get(entity);
                selectedView.Value.Hide();
                
                _selectedShowedStash.Remove(entity);
            }
            
            foreach (var entity in  _allUnselectedRequestedUnitsFilter)
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