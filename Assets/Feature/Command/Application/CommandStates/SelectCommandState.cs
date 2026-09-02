using System.Collections.Generic;
using Feature.Command.Application.CommandStates.Interfaces;
using Feature.Command.Application.Interfaces;
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
        [Inject] private readonly IUnitQueryFacade _query;
        [Inject] private readonly ISelectRectView _selectRectView;
        // [Inject] private readonly MovePreviewService _movePreview;
        [Inject] private readonly CommandButtonsVisual _commandButtonsVisual;

        private Vector2 _dragStart;
        private bool _isDragging;
        private bool _shiftHold;
        private bool _ctrlHold;
        private IReadOnlyList<Entity> _selectedUnits;

        public void SetShiftHold(bool shiftHold)
        {
            _shiftHold = shiftHold;

            if (shiftHold)
            {
                _ctrlHold = false;
            }

            _select.ClearAllSelectRequests();
        }

        public void SetCtrlHold(bool ctrlHold)
        {
            if (ctrlHold)
            {
                _shiftHold = false;
            }

            _select.ClearAllSelectRequests();
            _ctrlHold = ctrlHold;
        }

        public void OnEnter()
        {
            _commandButtonsVisual.SetActiveButton(CommandType.Select);
        }

        public void OnExit()
        {
            _isDragging = false;
            _selectRectView.Hide();
        }

        public void OnPointerDown(Vector2 pointerPos)
        {
            _dragStart = pointerPos;
            _isDragging = true;

            UpdateSelectionRect(pointerPos, out var selectRect);

            _selectRectView.Show();
        }


        public void OnPointerMove(Vector2 pointerPos)
        {
            if (!_isDragging) return;

            UpdateSelectionRect(pointerPos, out var selectRect);

            var dragDistance = (pointerPos - _dragStart).sqrMagnitude;
            var threshold = _selectionQuery.GetSingleSelectionThreshold();

            if (dragDistance <= threshold)
                return;

            _selectedUnits = _selectionQuery.QueryUnitsInRect(selectRect);

            if (_shiftHold)
            {
                _select.SelectPreview(_selectedUnits);
            }
            else if (_ctrlHold)
            {
                _select.UnselectPreview(_selectedUnits);
            }
            else
            {
                _select.SelectPreviewWithClear(_selectedUnits);
            }
        }

        private void UpdateSelectionRect(Vector2 screenPos, out Rect selectRect)
        {
            selectRect = BuildRect(_dragStart, screenPos);
            _selectRectView.UpdateSelectionRect(selectRect);
        }

        public void OnPointerUp(Vector2 pointerPos)
        {
            if (!_isDragging) return;
            _isDragging = false;

            _selectRectView.Hide();

            var dragDistance = (pointerPos - _dragStart).sqrMagnitude;
            var singleSelectionThreshold = _selectionQuery.GetSingleSelectionThreshold();

            if (dragDistance < singleSelectionThreshold)
            {
                var unit = _selectionQuery.GetUnitAtPoint(pointerPos, singleSelectionThreshold);

                if (unit.Count > 0)
                {
                    if (_shiftHold)
                    {
                        _select.ToggleSelectPreview(unit);
                    }
                    else if (_ctrlHold)
                    {
                        _select.UnselectPreview(unit);
                    }
                    else
                    {
                        _select.SelectPreviewWithClear(unit);
                    }
                }
                else
                {
                    if ((_shiftHold || _ctrlHold) == false)
                    {
                        _select.ClearAllSelected();
                    }
                }
            }

            //_movePreview.CalculateUnitsCenter(_selectedUnits);

            _select.CommitSelected();
        }

        private Rect BuildRect(Vector2 a, Vector2 b)
        {
            var min = Vector2.Min(a, b);
            var size = new Vector2(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
            return new Rect(min, size);
        }
    }
}