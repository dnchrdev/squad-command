using System;
using System.Collections.Generic;
using Feature.GameplayECS.Common;
using Feature.GameplayECS.Facade.Interfaces;
using Feature.GameplayECS.SelectCommand;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Facade
{
    public class UnitQueryFacade : IUnitQueryFacade
    {
        private readonly World _world;

        private readonly Filter _allUnitsFilter;
        private readonly Filter _previewSelectedFilter;
        private readonly Filter _selectedFilter;

        private readonly Stash<Position> _positionStash;

        private readonly List<Entity> _buffer = new List<Entity>();

        public UnitQueryFacade(World world)
        {
            _world = world;

            _allUnitsFilter = _world.Filter.With<UnitTag>().Build();
            _previewSelectedFilter = _world.Filter.With<UnitTag>().With<SelectSelfRequest>().Build();
            _selectedFilter = _world.Filter.With<UnitTag>().With<Selected>().Build();

            _positionStash = _world.GetStash<Position>();
        }

        public ref Position GetPosition(Entity entity) => ref _positionStash.Get(entity);

        public IReadOnlyList<Entity> GetAllUnits() => ToList(_allUnitsFilter);
        public IReadOnlyList<Entity> GetSelectedUnits() => ToList(_selectedFilter);
        public IReadOnlyList<Entity> GetPreviewSelectedUnits() => ToList(_previewSelectedFilter);

        public IReadOnlyList<Entity> GetUnitsInScreenRect(Camera camera, Rect screenRect)
        {
            _buffer.Clear();

            foreach (var entity in _allUnitsFilter)
            {
                ref var position = ref _positionStash.Get(entity);
                var screenPos = camera.WorldToScreenPoint(position.Value);

                if (screenPos.z < 0) continue;
                if (screenRect.Contains(screenPos)) _buffer.Add(entity);
            }

            return _buffer;
        }

        public Entity? GetUnitAtScreenPoint(Camera camera, Vector2 screenPoint, float tolerance)
        {
            foreach (var entity in _allUnitsFilter)
            {
                ref var position = ref _positionStash.Get(entity);
                var screenPos = camera.WorldToScreenPoint(position.Value);

                if (screenPos.z < 0) continue;

                if (Vector2.Distance(screenPos, screenPoint) < tolerance)
                    return entity;
            }

            return null;
        }

        public Vector3 GetUnitsCenter(IReadOnlyList<Entity> units)
        {
            if (units.Count == 0) return Vector3.zero;

            Vector3 sum = Vector3.zero;
            for (int i = 0; i < units.Count; i++)
                sum += _positionStash.Get(units[i]).Value;

            return sum / units.Count;
        }

        private List<Entity> ToList(Filter filter)
        {
            _buffer.Clear();
            foreach (var entity in filter) _buffer.Add(entity);
            return _buffer;
        }
    }
}