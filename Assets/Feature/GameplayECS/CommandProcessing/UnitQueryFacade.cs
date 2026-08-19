using Feature.GameplayECS.Common;
using Feature.GameplayECS.Navigation;
using Feature.GameplayECS.Select;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.CommandProcessing
{
    public class UnitQueryFacade: IUnitQueryFacade
    {
        private readonly World _world;

        private readonly Filter _allUnitsFilter;
        private readonly Filter _previewSelectedFilter;
        
        private readonly Stash<Position> _positionStash;

        public Filter AllUnits => _allUnitsFilter;
        public Filter PreviewSelectedUnits => _previewSelectedFilter;
        
        public UnitQueryFacade(World world)
        {
            _world = world;
            _allUnitsFilter = _world.Filter.With<UnitTag>().Build();
            _previewSelectedFilter = _world.Filter.With<UnitTag>().With<SelectSelfRequest>().Build();
            
            _positionStash = _world.GetStash<Position>();
        }
        
        public ref Position GetPosition(Entity entity)
        {
            return ref _positionStash.Get(entity);
        }
    }
}