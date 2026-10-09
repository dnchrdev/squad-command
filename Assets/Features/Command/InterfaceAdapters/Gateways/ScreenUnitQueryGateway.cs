using System;
using System.Collections.Generic;
using Features.Command.UseCases.Ports;
using Features.Shared.Projection;
using Features.Shared.Units;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Command.InterfaceAdapters.Gateways
{
    public sealed class ScreenUnitQueryGateway : IScreenUnitQuery
    {
        private readonly IScreenProjection _projection;
        private readonly IUnitQuery _unitQuery;
        private readonly List<UnitSnapshot> _unitsBuffer = new();
        private readonly List<Entity> _resultBuffer = new();

        public ScreenUnitQueryGateway(IScreenProjection projection, IUnitQuery unitQuery)
        {
            _projection = projection;
            _unitQuery = unitQuery;
        }

        public IReadOnlyList<Entity> GetUnitAtPoint(Vector2 screenPoint, float tolerance)
        {
            _unitQuery.GetAllUnits(_unitsBuffer);

            foreach (var unit in _unitsBuffer)
            {
                var screenPos = _projection.WorldToScreenPoint(unit.Position);
                if (screenPos.z < 0) continue;

                if (Vector2.Distance(screenPos, screenPoint) < tolerance)
                    return new List<Entity> { unit.Entity };
            }

            return Array.Empty<Entity>();
        }

        public IReadOnlyList<Entity> GetUnitsInRect(Rect screenRect)
        {
            _unitQuery.GetAllUnits(_unitsBuffer);
            _resultBuffer.Clear();

            foreach (var unit in _unitsBuffer)
            {
                var screenPos = _projection.WorldToScreenPoint(unit.Position);
                if (screenPos.z < 0) continue;

                if (screenRect.Contains(screenPos))
                    _resultBuffer.Add(unit.Entity);
            }

            return _resultBuffer;
        }
    }
}