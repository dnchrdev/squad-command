using System;
using System.Collections.Generic;
using Features.Command.UseCases.Ports;
using Scellecs.Morpeh;
using UnityEngine;

namespace Tests.EditMode.Command.UseCases.Fakes
{
    public sealed class StubScreenUnitQuery : IScreenUnitQuery
    {
        public IReadOnlyList<Entity> UnitsAtPoint = Array.Empty<Entity>();
        public IReadOnlyList<Entity> UnitsInRect = Array.Empty<Entity>();

        public float LastTolerance;

        public IReadOnlyList<Entity> GetUnitAtPoint(Vector2 screenPoint, float tolerance)
        {
            LastTolerance = tolerance;
            return UnitsAtPoint;
        }

        public IReadOnlyList<Entity> GetUnitsInRect(Rect screenRect) => UnitsInRect;
    }
}
