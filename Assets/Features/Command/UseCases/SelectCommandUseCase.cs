using System.Collections.Generic;
using Features.Command.Entities;
using Features.Command.UseCases.Ports;
using Features.Shared.Units;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Command.UseCases
{
    public sealed class SelectCommandUseCase : ICommandState
    {
        private readonly IScreenUnitQuery _unitQuery;
        private readonly ISelectionCommands _select;
        private readonly ISelectionFrameOutput _frame;
        private readonly ICommandModeOutput _mode;
        private readonly CommandSettings _settings;

        private readonly SelectionModifiers _modifiers = new();
        private Vector2 _dragStart;
        private bool _isDragging;

        public SelectCommandUseCase(
            IScreenUnitQuery unitQuery,
            ISelectionCommands select,
            ISelectionFrameOutput frame,
            ICommandModeOutput mode,
            CommandSettings settings)
        {
            _unitQuery = unitQuery;
            _select = select;
            _frame = frame;
            _mode = mode;
            _settings = settings;
        }

        public void SetShiftHold(bool shiftHold)
        {
            _modifiers.SetShift(shiftHold);
            _select.ClearSelectionPreview();
        }

        public void SetCtrlHold(bool ctrlHold)
        {
            _modifiers.SetCtrl(ctrlHold);
            _select.ClearSelectionPreview();
        }

        public void OnEnter() => _mode.ShowActiveCommand(CommandType.SELECT);

        public void OnExit()
        {
            _isDragging = false;
            _frame.HideFrame();

            if (_isDragging)
                _select.ClearSelection();
        }

        public void OnPointerDown(Vector2 pointerPos)
        {
            _dragStart = pointerPos;
            _isDragging = true;
            _frame.ShowFrame(BuildRect(pointerPos, pointerPos));
        }

        public void OnPointerMove(Vector2 pointerPos)
        {
            if (_isDragging == false) return;

            var rect = BuildRect(_dragStart, pointerPos);
            _frame.UpdateFrame(rect);

            if ((pointerPos - _dragStart).sqrMagnitude <= _settings.SingleSelectThreshold)
                return;

            ApplySelection(_unitQuery.GetUnitsInRect(rect));
        }

        public void OnPointerUp(Vector2 pointerPos)
        {
            if (_isDragging == false) return;

            _isDragging = false;
            _frame.HideFrame();

            var threshold = _settings.SingleSelectThreshold;
            if ((pointerPos - _dragStart).sqrMagnitude < threshold)
            {
                var unit = _unitQuery.GetUnitAtPoint(pointerPos, threshold);
                if (unit.Count > 0)
                    ApplySelection(unit);
                else if (_modifiers.Mode == SelectionMode.REPLACE)
                    _select.ClearSelection();
            }

            _select.CommitSelection();
        }

        public void Canceled()
        {
            _isDragging = false;

            _frame.HideFrame();

            if (_isDragging)
                _select.ClearSelection();
        }

        private void ApplySelection(IReadOnlyList<Entity> units)
        {
            switch (_modifiers.Mode)
            {
                case SelectionMode.ADD: _select.AddToSelectionPreview(units); break;
                case SelectionMode.REMOVE: _select.RemoveFromSelectionPreview(units); break;
                default: _select.ReplaceSelectionPreview(units); break;
            }
        }

        private static Rect BuildRect(Vector2 a, Vector2 b)
        {
            var min = Vector2.Min(a, b);
            return new Rect(min, new Vector2(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y)));
        }
    }
}