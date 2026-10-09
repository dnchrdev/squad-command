using System.Collections.Generic;
using Scellecs.Morpeh;
using Rect = UnityEngine.Rect;
using Vector2 = UnityEngine.Vector2;

namespace Features.Command.UseCases.Ports
{
    public interface IScreenUnitQuery
    {
        IReadOnlyList<Entity> GetUnitAtPoint(Vector2 screenPoint, float tolerance);
        IReadOnlyList<Entity> GetUnitsInRect(Rect screenRect);
    }
}