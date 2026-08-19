using Feature.GameplayECS.Common;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.Select.Systems
{
    public class ShowSelectedSelfPreviewSystem: ISystem
    {
        public World World { get; set; }

        private Filter _selectSelfUnitsFilter;
        private Stash<SelectedViewShowed> _selectedShowedStash;
        private Stash<SelectViewComponent> _selectViewStash;
        
        public void OnAwake()
        {
            _selectSelfUnitsFilter = World.Filter
                .With<UnitTag>()
                .With<SelectSelfRequest>()
                .With<SelectViewComponent>()
                .Without<SelectedViewShowed>()
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
                
                _selectedShowedStash.Add(entity);
            }
            
        }

        public void Dispose()
        {
 
        }
    }
}