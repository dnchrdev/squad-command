using Feature.Command.Adapter.Interfaces;
using Feature.Command.Application.CursorModes.Interfaces;
using Feature.Command.Infrastructure.Interfaces;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;
using Zenject;

namespace Feature.Command.Application.CursorModes
{
    public sealed class SelectCursorMode : ICursorMode
    {
        [Inject] private readonly ISelectionQueryService _selectionQuery;
        [Inject] private readonly SelectionUseCase _selectionUseCase;
        [Inject] private readonly ISelectRectView _selectRectView;

        private Vector2 _dragStart;
        private bool _isDragging;
        private int _selectedUnitsCount;

        public void OnEnter() { }
        public void OnExit()
        {
            _isDragging = false;
            _selectRectView.Hide();
        }

        public void OnPointerDown(Vector2 screenPos)
        {
            _dragStart = screenPos;
            _isDragging = true;
            
            UpdateSelectionRect(screenPos, out var selectRect);

            _selectRectView.Show();
        }


        public void OnPointerMove(Vector2 screenPos)
        {
            if (!_isDragging) return;

            UpdateSelectionRect(screenPos, out var selectRect);

            var dragDistance = (screenPos - _dragStart).sqrMagnitude;
            var threshold = _selectionQuery.GetSingleSelectionThreshold();
            
            if (dragDistance <= threshold)
                return;
            
            var units = _selectionQuery.QueryUnitsInRect(selectRect);
            _selectedUnitsCount = units.Count;
            _selectionUseCase.SelectMany(units);
        }

        private void UpdateSelectionRect(Vector2 screenPos, out Rect selectRect)
        {
            selectRect = BuildRect(_dragStart, screenPos);
            _selectRectView.UpdateSelectionRect(selectRect);
        }
        
        public void OnPointerUp(Vector2 screenPos)
        {
            if (!_isDragging) return;
            _isDragging = false;
                    
            _selectRectView.Hide();
            
            var dragDistance = (screenPos - _dragStart).sqrMagnitude;
            var threshold = _selectionQuery.GetSingleSelectionThreshold();
            
            if (dragDistance > threshold)
            {
                if(_selectedUnitsCount < 1)
                {
                    _selectionUseCase.Clear();
                    return;
                }
            }
            else 
            if (_selectionQuery.TryGetUnitAtPoint(screenPos, out var entity))
            {
                _selectionUseCase.SelectSingle(entity);
            }

            _selectionUseCase.CommitSelected();
        }

        private Rect BuildRect(Vector2 a, Vector2 b)
        {
            var min = Vector2.Min(a, b);
            var size = new Vector2(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));
            return new Rect(min, size);
        }
    }
}