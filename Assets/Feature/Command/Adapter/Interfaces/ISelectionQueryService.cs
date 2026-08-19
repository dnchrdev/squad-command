using System.Collections.Generic;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.Command.Adapter.Interfaces
{
    public interface ISelectionQueryService
    {
        float GetSingleSelectionThreshold();
        bool TryGetUnitAtPoint(Vector2 screenPoint, out Entity entity);
        IReadOnlyList<Entity> QueryUnitsInRect(Rect screenRect);
        
    }
}