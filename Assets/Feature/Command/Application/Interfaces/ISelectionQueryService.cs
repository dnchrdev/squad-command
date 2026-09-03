using System.Collections.Generic;
using Scellecs.Morpeh;
using Vector2 = UnityEngine.Vector2;
using Rect = UnityEngine.Rect;

namespace Feature.Command.Application.Interfaces
{
    public interface ISelectionQueryService
    {
        float GetSingleSelectionThreshold();
        IReadOnlyList<Entity> GetUnitAtPoint(Vector2 pointerPosition, float tolerance);
        IReadOnlyList<Entity> QueryUnitsInRect(Rect screenRect);
    }
}