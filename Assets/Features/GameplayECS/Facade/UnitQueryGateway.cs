using System.Collections.Generic;
using Features.GameplayECS.Common.Components;
using Features.Shared.Units;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Facade
{
    public sealed class UnitQueryGateway : IUnitQuery
    {
        private readonly Filter _allUnitsFilter;
        private readonly Filter _selectedUnitsFilter;
        private readonly Stash<Position> _positionStash;

        public UnitQueryGateway(World world)
        {
            _allUnitsFilter = world.Filter.With<UnitTag>().With<Position>().Build();
            _selectedUnitsFilter = world.Filter.With<UnitTag>().With<Position>().With<SelectedTag>().Build();
            _positionStash = world.GetStash<Position>();
        }

        public void GetAllUnits(List<UnitSnapshot> result) => Fill(_allUnitsFilter, result);

        public void GetSelectedUnits(List<UnitSnapshot> result) => Fill(_selectedUnitsFilter, result);

        private void Fill(Filter filter, List<UnitSnapshot> result)
        {
            result.Clear();
            foreach (var entity in filter)
                result.Add(new UnitSnapshot(entity, _positionStash.Get(entity).Value));
        }
    }
}