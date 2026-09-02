using System.Collections.Generic;
using Feature.CameraFeature.Infrastructure;
using Feature.CameraFeature.Infrastructure.Interfaces;
using Feature.Command.Application.Interfaces;
using Feature.Command.Infrastructure.Configs;
using Feature.GameplayECS.Facade.Interfaces;
using Scellecs.Morpeh;
using UnityEngine;
using Zenject;

namespace Feature.Command.Adapter
{
    public class SelectionQueryService : ISelectionQueryService
    {
        [Inject] private readonly CommandConfig _commandConfig;
        [Inject] private readonly IReadOnlyCamera _camera;
        [Inject] private readonly IUnitQueryFacade _queryFacade;

        public float GetSingleSelectionThreshold() => _commandConfig.SingleSelectMaxThreshold;

        public IReadOnlyList<Entity> GetUnitAtPoint(Vector2 pointerPosition, float tolerance)
        {
            var entity = _queryFacade.GetUnitAtScreenPoint(_camera.Camera, pointerPosition, tolerance);
            return entity.HasValue
                ? new List<Entity> { entity.Value }
                : System.Array.Empty<Entity>();
        }

        public IReadOnlyList<Entity> QueryUnitsInRect(Rect screenRect)
        {
            return _queryFacade.GetUnitsInScreenRect(_camera.Camera, screenRect);
        }
    }
}