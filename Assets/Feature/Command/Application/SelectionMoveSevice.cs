using System.Collections.Generic;
using Feature.GameplayECS.Facade;
using Feature.GameplayECS.Facade.Interfaces;
using Scellecs.Morpeh;
using UnityEngine;
using Zenject;

namespace Feature.Command.Application
{
    public class MovePreviewService
    {
        [Inject] private readonly IUnitQueryFacade _query;

        private Vector2 _destinationPoint;
        private Vector2 _selectedUnitsCenter;
        private Vector2 _formationForwardDirection;
        private Vector2 _formationRightDirection;
        private Vector2 _projectedFormationCenter;
        private Vector2 _selectionSize;

        public Vector2 SelectedUnitsCenter => _selectedUnitsCenter;
        public Vector2 SelectionSize => _selectionSize;
        public Vector2 ProjectedFormationCenter => _projectedFormationCenter;
        public Vector2 FormationForwardDirection => _formationForwardDirection;
        public Vector2 FormationRightDirection => _formationRightDirection;

        public void CalculateUnitsCenter(IReadOnlyList<Entity> units)
        {
            Vector3 center = FindFormationCenter(units);
            _selectedUnitsCenter = new Vector2(center.x, center.z);
        }
        
        public void CalculateSelectedUnitsCenter()
        {
            Vector3 sumPosition = Vector3.zero;
            int count = 0;
            
            foreach (var unit in _query.SelectedUnits)
            {
                    ref var position = ref _query.GetPosition(unit);
                    sumPosition += position.Value;
                    count++;
            }
            Vector3 center = sumPosition * 1.0f / count;
            _selectedUnitsCenter = new Vector2(center.x, center.z);
        }

        public void SetMoveDirectionStartPoint(Vector3 worldPosition)
        {
            _selectedUnitsCenter = new Vector2(worldPosition.x, worldPosition.z);
        }

        public void SetProjectedFormationCenter(Vector3 startWorldPosition)
        {
            _projectedFormationCenter = new Vector2(startWorldPosition.x, startWorldPosition.z);
        }
        
        public void CalculateSelectionSize(Vector3 startWorldPosition, Vector3 endWorldPosition)
        {
            _formationRightDirection = new Vector2(_formationForwardDirection.y, -_formationForwardDirection.x);

            Vector2 startFlat = new Vector2(startWorldPosition.x, startWorldPosition.z);
            Vector2 endFlat = new Vector2(endWorldPosition.x, endWorldPosition.z);
            Vector2 delta = startFlat - endFlat;

            float forwardProjectionSize = Mathf.Abs(Vector2.Dot(delta, _formationForwardDirection));
            float rightProjectionSize = Mathf.Abs(Vector2.Dot(delta, _formationRightDirection));
            _selectionSize = new Vector2(rightProjectionSize, forwardProjectionSize);
        }

        public void SetFormationDirection(Vector3 newDirection)
        {
             Vector3 direction = (Vector3.ProjectOnPlane(newDirection, Vector3.up)).normalized;
             _formationForwardDirection = new Vector2(direction.x, direction.z);
        }
        
        public void SetFormationDirection(Vector2 newDirection)
        {
            _formationForwardDirection = newDirection;
        }

        private Vector3 FindFormationCenter(IReadOnlyList<Entity> units)
        {
            Vector3 sumPosition = Vector3.zero;

            for (int i = 0; i < units.Count; i++)
            {
                ref var position = ref _query.GetPosition(units[i]);
                sumPosition += position.Value;
            }

            Vector3 center = sumPosition * 1.0f / units.Count;
            return center;
        }
    }
}