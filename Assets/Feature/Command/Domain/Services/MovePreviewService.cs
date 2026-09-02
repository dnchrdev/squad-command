using System.Collections.Generic;
using UnityEngine;

namespace Feature.Command.Domain.Services
{
    public class MovePreviewService
    {
        public Vector2 SelectedUnitsCenter { get; private set; }
        public Vector2 SelectionSize { get; private set; }
        public Vector2 ProjectedFormationCenter { get; private set; }
        public Vector2 FormationForwardDirection { get; private set; }
        public Vector2 FormationRightDirection { get; private set; }

        /// <summary>
        /// Считает центр отряда по уже полученным извне мировым позициям юнитов.
        /// </summary>
        public void CalculateUnitsCenter(IReadOnlyList<Vector3> unitWorldPositions)
        {
            Vector3 center = FindFormationCenter(unitWorldPositions);
            SelectedUnitsCenter = new Vector2(center.x, center.z);
        }

        public void SetMoveDirectionStartPoint(Vector3 worldPosition)
        {
            SelectedUnitsCenter = new Vector2(worldPosition.x, worldPosition.z);
        }

        public void SetProjectedFormationCenter(Vector3 startWorldPosition)
        {
            ProjectedFormationCenter = new Vector2(startWorldPosition.x, startWorldPosition.z);
        }

        public void CalculateSelectionSize(Vector3 startWorldPosition, Vector3 endWorldPosition)
        {
            FormationRightDirection = new Vector2(FormationForwardDirection.y, -FormationForwardDirection.x);

            Vector2 startFlat = new Vector2(startWorldPosition.x, startWorldPosition.z);
            Vector2 endFlat = new Vector2(endWorldPosition.x, endWorldPosition.z);
            Vector2 delta = startFlat - endFlat;

            float forwardProjectionSize = Mathf.Abs(Vector2.Dot(delta, FormationForwardDirection));
            float rightProjectionSize = Mathf.Abs(Vector2.Dot(delta, FormationRightDirection));
            SelectionSize = new Vector2(rightProjectionSize, forwardProjectionSize);
        }

        public void SetFormationDirection(Vector3 newDirection)
        {
            Vector3 direction = Vector3.ProjectOnPlane(newDirection, Vector3.up).normalized;
            FormationForwardDirection = new Vector2(direction.x, direction.z);
        }

        public void SetFormationDirection(Vector2 newDirection)
        {
            FormationForwardDirection = newDirection;
        }

        private Vector3 FindFormationCenter(IReadOnlyList<Vector3> positions)
        {
            if (positions.Count == 0) return Vector3.zero;

            Vector3 sum = Vector3.zero;
            for (int i = 0; i < positions.Count; i++)
                sum += positions[i];

            return sum / positions.Count;
        }
    }
}