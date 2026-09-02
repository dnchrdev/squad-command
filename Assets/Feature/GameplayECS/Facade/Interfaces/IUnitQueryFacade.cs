using System.Collections.Generic;
using Feature.GameplayECS.Common;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Facade.Interfaces
{
    public interface IUnitQueryFacade
    {
        // --- Позиция ---
        ref Position GetPosition(Entity entity);

        // --- Готовые наборы юнитов (без утечки Filter наружу) ---
        IReadOnlyList<Entity> GetAllUnits();
        IReadOnlyList<Entity> GetSelectedUnits();
        IReadOnlyList<Entity> GetPreviewSelectedUnits();

        // --- Пространственные запросы (то, что раньше руками делал SelectionQueryService) ---
        IReadOnlyList<Entity> GetUnitsInScreenRect(Camera camera, Rect screenRect);
        Entity? GetUnitAtScreenPoint(Camera camera, Vector2 screenPoint, float tolerance);

        // --- Вычисления над набором юнитов ---
        Vector3 GetUnitsCenter(IReadOnlyList<Entity> units);
    }
}