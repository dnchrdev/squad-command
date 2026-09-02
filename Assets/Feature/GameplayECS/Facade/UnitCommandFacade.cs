using Feature.GameplayECS.Common;
using Feature.GameplayECS.Facade.Interfaces;
using Feature.GameplayECS.SelectCommand;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.Facade
{
    public class UnitQueryFacade: IUnitQueryFacade
    {
        private readonly World _world;

        private readonly Filter _allUnitsFilter;
        private readonly Filter _previewSelectedFilter;
        private readonly Filter _selectedFilter;
        
        private readonly Stash<Position> _positionStash;

        public Filter AllUnits => _allUnitsFilter;
        public Filter PreviewSelectedUnits => _previewSelectedFilter;
        public Filter SelectedUnits => _selectedFilter;
        
        public UnitQueryFacade(World world)
        {
            _world = world;
            _allUnitsFilter = _world.Filter.With<UnitTag>().Build();
            _previewSelectedFilter = _world.Filter.With<UnitTag>().With<SelectSelfRequest>().Build();
            _selectedFilter = _world.Filter.With<UnitTag>().With<Selected>().Build();
            
            _positionStash = _world.GetStash<Position>();
        }
        
        public ref Position GetPosition(Entity entity)
        {
            return ref _positionStash.Get(entity);
        }
    }
}