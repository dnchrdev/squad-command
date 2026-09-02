using System.Collections.Generic;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.Command.Application.Interfaces
{
    public interface ISelectionQueryService
    {
        float GetSingleSelectionThreshold();
        IReadOnlyList<Entity> GetUnitAtPoint(Vector2 pointerPosition, float tolerance);
        IReadOnlyList<Entity> QueryUnitsInRect(Rect screenRect);
        
    }
}