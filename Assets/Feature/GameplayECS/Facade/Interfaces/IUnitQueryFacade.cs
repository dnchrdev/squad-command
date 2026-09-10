using System.Collections.Generic;
using Feature.GameplayECS.Common;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Facade.Interfaces
{
    public interface IUnitQueryFacade
    {
        ref Position GetPosition(Entity entity);
        
        IReadOnlyList<Entity> GetAllUnits();
        IReadOnlyList<Entity> GetSelectedUnits();
        IReadOnlyList<Entity> GetPreviewSelectedUnits();
        
        IReadOnlyList<Entity> GetUnitsInScreenRect(Camera camera, Rect screenRect);
        Entity? GetUnitAtScreenPoint(Camera camera, Vector2 screenPoint, float tolerance);

        Vector3 GetUnitsCenter(IReadOnlyList<Entity> units);
    }
}