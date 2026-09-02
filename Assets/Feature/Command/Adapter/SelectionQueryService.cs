using System.Collections.Generic;
using Feature.CameraFeature.Infrastructure;
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
        [Inject] private ISelectCommandFacade _selectFacade;
        [Inject] private IUnitQueryFacade _queryFacade;

        private List<Entity> _unitsInRect = new List<Entity>();

        public float GetSingleSelectionThreshold() => _commandConfig.SingleSelectMaxThreshold;

        public IReadOnlyList<Entity> GetUnitAtPoint(Vector2 pointerPosition, float tolerance)
        {
            _unitsInRect.Clear();
            
            foreach (var unit in _queryFacade.AllUnits)
            {
                ref var position = ref _queryFacade.GetPosition(unit);
                var unitScreenPosition =
                    _camera.Camera.WorldToScreenPoint(position.Value);

                if (unitScreenPosition.z < 0)
                    continue;

                if (Vector2.Distance(unitScreenPosition, pointerPosition) < tolerance)
                {
                    _unitsInRect.Add(unit);
                    return _unitsInRect;
                }
            }

            return _unitsInRect;
        }

        public IReadOnlyList<Entity> QueryUnitsInRect(Rect screenRect)
        {
            _unitsInRect.Clear();

            foreach (var entity in _queryFacade.AllUnits)
            {
                ref var position = ref _queryFacade.GetPosition(entity);

                var screenPosition =
                    _camera.Camera.WorldToScreenPoint(position.Value);

                if (screenPosition.z < 0)
                    continue;

                if (screenRect.Contains(screenPosition))
                    _unitsInRect.Add(entity);
            }

            return _unitsInRect;
        }
    }
}