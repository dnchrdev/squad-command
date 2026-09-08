using System.Collections.Generic;
using Feature.Command.Application.CommandStates.Interfaces;
using Feature.Command.Application.Interfaces;
using Feature.Command.Domain;
using Feature.Command.Domain.Data;
using Feature.Command.Domain.Services;
using Feature.GameplayECS.Facade.Interfaces;
using Scellecs.Morpeh;
using Zenject;
using Vector2 = UnityEngine.Vector2;
using Rect = UnityEngine.Rect;
using Mathf = UnityEngine.Mathf;

namespace Feature.Command.Application.CommandStates
{
    public sealed class SelectCommandState : ICommandState
    {
        [Inject] private readonly ISelectionQueryService _selectionQuery;
        [Inject] private readonly ISelectCommandFacade _select;
        [Inject] private readonly ISelectRectView _selectRectView;
        [Inject] private readonly CommandButtonsVisual _commandButtonsVisual;

        private readonly SelectionModifiers _modifiers = new();
        private Vector2 _dragStart;
        private bool _isDragging;

        public void SetShiftHold(bool shiftHold)
        {
            _modifiers.SetShift(shiftHold);
            _select.ClearAllSelectRequests();
        }

        public void SetCtrlHold(bool ctrlHold)
        {
            _modifiers.SetCtrl(ctrlHold);
            _select.ClearAllSelectRequests();
        }

        public void OnEnter() => _commandButtonsVisual.SetActiveButton(CommandType.Select);

        public void OnExit()
        {
            _isDragging = false;
            _selectRectView.Hide();

            if (_isDragging)
                _select.ClearAllSelected();
        }

        public void OnPointerDown(Vector2 pointerPos)
        {
            _dragStart = pointerPos;
            _isDragging = true;
            _selectRectView.UpdateSelectionRect(BuildRect(pointerPos, pointerPos));
            _selectRectView.Show();
        }

        public void OnPointerMove(Vector2 pointerPos)
        {
            if (_isDragging == false) return;

            var rect = BuildRect(_dragStart, pointerPos);
            _selectRectView.UpdateSelectionRect(rect);

            if ((pointerPos - _dragStart).sqrMagnitude <= _selectionQuery.GetSingleSelectionThreshold())
                return;

            ApplySelection(_selectionQuery.QueryUnitsInRect(rect));
        }

        public void OnPointerUp(Vector2 pointerPos)
        {
            if (_isDragging == false) return;
            
            _isDragging = false;
            _selectRectView.Hide();

            var threshold = _selectionQuery.GetSingleSelectionThreshold();
            if ((pointerPos - _dragStart).sqrMagnitude < threshold)
            {
                var unit = _selectionQuery.GetUnitAtPoint(pointerPos, threshold);
                if (unit.Count > 0)
                    ApplySelection(unit);
                else if (_modifiers.Mode == SelectionMode.Replace)
                    _select.ClearAllSelected();
            }

            _select.CommitSelected();
        }

        public void Canceled()
        {
            _isDragging = false;

            _selectRectView.Hide();

            if (_isDragging)
                _select.ClearAllSelected();
        }

        private void ApplySelection(IReadOnlyList<Entity> units)
        {
            switch (_modifiers.Mode)
            {
                case SelectionMode.Add: _select.SelectPreview(units); break;
                case SelectionMode.Remove: _select.UnselectPreview(units); break;
                default: _select.SelectPreviewWithClear(units); break;
            }
        }

        private Rect BuildRect(Vector2 a, Vector2 b)
        {
            var min = Vector2.Min(a, b);
            return new Rect(min, new Vector2(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y)));
        }
    }
}