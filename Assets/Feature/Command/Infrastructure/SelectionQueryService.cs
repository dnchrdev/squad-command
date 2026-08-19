using System.Collections.Generic;
using Feature.CameraFeature.Infrastructure;
using Feature.Command.Adapter.Interfaces;
using Feature.Command.Domain;
using Feature.Command.Infrastructure.Configs;
using Feature.GameplayECS.CommandProcessing;
using Feature.GameplayECS.View;
using Scellecs.Morpeh;
using UnityEngine;
using Zenject;

namespace Feature.Command.Infrastructure
{
    public class SelectionQueryService : ISelectionQueryService
    {
        [Inject] private readonly CommandConfig _commandConfig;
        [Inject] private readonly IReadOnlyCamera _camera;
        [Inject] private ICommandSquadFacade _commandFacade;
        [Inject] private IUnitQueryFacade _queryFacade;

        private List<Entity> _unitsInRect =  new List<Entity>();
        
        public float GetSingleSelectionThreshold() => _commandConfig.SingleSelectMaxThreshold;

        public bool TryGetUnitAtPoint(Vector2 screenPoint, out Entity entity)
        {
            entity = default;
            var ray = _camera.Camera.ScreenPointToRay(screenPoint);

            var hasHit = Physics.SphereCast(ray.origin, _commandConfig.SingleSelectCastRadius, ray.direction,
                out var hit,
                _commandConfig.SingleSelectCastDistance, _commandConfig.UnitMask, QueryTriggerInteraction.Collide);

            if (!hasHit) return false;

            var view = hit.collider.GetComponent<MonoEntity>();
            if (view == null)
            {
                return false;
            }

            Debug.Log("Hit");
            entity = view.Entity;
            return true;
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