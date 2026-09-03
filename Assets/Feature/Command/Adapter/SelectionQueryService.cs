using System;
using System.Collections.Generic;
using Feature.CameraFeature.Infrastructure.Interfaces;
using Feature.Command.Application.Interfaces;
using Feature.GameplayECS.Facade.Interfaces;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.Command.Adapter
{
    public class SelectionQueryService : ISelectionQueryService
    {
        private readonly float _singleSelectThreshold;
        private readonly IReadOnlyCamera _camera;
        private readonly IUnitQueryFacade _queryFacade;

        public SelectionQueryService(float singleSelectThreshold, IReadOnlyCamera camera, IUnitQueryFacade queryFacade)
        {
            _singleSelectThreshold = singleSelectThreshold;
            _camera = camera;
            _queryFacade = queryFacade;
        }

        public float GetSingleSelectionThreshold() => _singleSelectThreshold;

        public IReadOnlyList<Entity> GetUnitAtPoint(Vector2 pointerPosition, float tolerance)
        {
            var entity = _queryFacade.GetUnitAtScreenPoint(_camera.Camera, pointerPosition, tolerance);
            return entity.HasValue ? new List<Entity> { entity.Value } : Array.Empty<Entity>();
        }

        public IReadOnlyList<Entity> QueryUnitsInRect(Rect screenRect)
            => _queryFacade.GetUnitsInScreenRect(_camera.Camera, screenRect);
    }
}