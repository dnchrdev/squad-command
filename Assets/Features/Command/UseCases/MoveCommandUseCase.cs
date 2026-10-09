using System.Collections.Generic;
using Features.Command.Entities;
using Features.Command.UseCases.Ports;
using Features.Shared.Math;
using Features.Shared.Units;
using UnityEngine;

namespace Features.Command.UseCases
{
    public sealed class MoveCommandUseCase : ICommandState
    {
        private readonly IUnitQuery _unitQuery;
        private readonly IMoveDragOutput _drag;
        private readonly IGroundQuery _groundQuery;
        private readonly IMoveCommands _move;
        private readonly ICommandModeOutput _mode;

        private readonly List<UnitSnapshot> _selectedUnits = new();
        private Formation _formation;
        private Vector2 _dragStartScreenPos;
        private Vector3 _dragStartWorldPos;
        private Vector3 _dragEndValidWorldPos;
        private bool _isDragging;

        public MoveCommandUseCase(
            IUnitQuery unitQuery,
            IMoveDragOutput drag,
            IGroundQuery groundQuery,
            IMoveCommands move,
            ICommandModeOutput mode)
        {
            _unitQuery = unitQuery;
            _drag = drag;
            _groundQuery = groundQuery;
            _move = move;
            _mode = mode;
        }

        public void SetShiftHold(bool shiftHold)
        {
        }

        public void SetCtrlHold(bool ctrlHold)
        {
        }

        public void OnEnter() => _mode.ShowActiveCommand(CommandType.MOVE);

        public void OnExit()
        {
            _drag.HideDrag();
            _isDragging = false;
            _move.ClearPreview();
        }

        public void OnPointerDown(Vector2 pointerPos)
        {
            _dragStartScreenPos = pointerPos;

            if (!_groundQuery.TryGetGroundPoint(pointerPos, out var worldPoint))
            {
                _isDragging = false;
                return;
            }

            _dragStartWorldPos = worldPoint;
            _dragEndValidWorldPos = worldPoint;
            _isDragging = true;

            _unitQuery.GetSelectedUnits(_selectedUnits);
            var unitsCenter = MathExtensions.ToFlat2(AverageWorldPosition(_selectedUnits));
            _formation = new Formation(unitsCenter, MathExtensions.ToFlat2(worldPoint));

            _drag.ShowDrag(pointerPos);
            _move.StartPreview();
            RequestUpdatePreview();
        }

        public void OnPointerMove(Vector2 pointerPos)
        {
            if (_isDragging == false) return;
            if (_groundQuery.TryGetGroundPoint(pointerPos, out var worldPoint))
                _dragEndValidWorldPos = worldPoint;

            _drag.UpdateDrag(_dragStartScreenPos, pointerPos);
            _formation.UpdateDrag(MathExtensions.ToFlat2(_dragStartWorldPos),
                MathExtensions.ToFlat2(_dragEndValidWorldPos));
            RequestUpdatePreview();
        }

        public void OnPointerUp(Vector2 pointerPos)
        {
            if (_isDragging == false) return;

            _isDragging = false;
            _drag.HideDrag();
            _unitQuery.GetSelectedUnits(_selectedUnits);
            Vector3 selectedUnitsCentroid = AverageWorldPosition(_selectedUnits);
            _move.CommitMove(_formation.GetDirectionToCenter(MathExtensions.ToFlat2(selectedUnitsCentroid)));
        }

        public void Canceled()
        {
            _drag.HideDrag();
            _isDragging = false;
            _move.ClearPreview();
        }

        private void RequestUpdatePreview()
            => _move.UpdatePreview(_formation.Center, _formation.Size, _formation.Forward, _formation.Right);

        private static Vector3 AverageWorldPosition(IReadOnlyList<UnitSnapshot> units)
        {
            if (units.Count == 0) return Vector3.zero;

            Vector3 sum = Vector3.zero;
            for (int i = 0; i < units.Count; i++)
                sum += units[i].Position;
            return sum / units.Count;
        }
    }
}