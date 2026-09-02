using System.Collections.Generic;
using Feature.Command.Application.CommandStates.Interfaces;
using Feature.Command.Application.Interfaces;
using Feature.Command.Domain.Data;
using Feature.Command.Domain.Services;
using Feature.GameplayECS.Facade.Interfaces;
using Zenject;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Feature.Command.Application.CommandStates
{
    public sealed class MoveCommandState : ICommandState
    {
        [Inject] private readonly IUnitQueryFacade _unitQuery;
        [Inject] private readonly IMoveVisualView _view;
        [Inject] private readonly IGroundQueryService _groundQuery;
        [Inject] private readonly IMoveCommandFacade _move;
        [Inject] private readonly MovePreviewService _movePreviewService;
        [Inject] private readonly CommandButtonsVisual _commandButtonsVisual;

        private readonly List<Vector3> _selectedUnitsPositionsBuffer = new();

        private Vector2 _dragStartScreenPos;
        private Vector2 _dragEndScreenPos;
        private Vector3 _dragStartWorldPos;
        private Vector3 _dragEndValidWorldPos;
        private bool _isDragging;

        public void SetShiftHold(bool shiftHold) { }
        public void SetCtrlHold(bool ctrlHold) { }

        public void OnEnter()
        {
            _move.ShowAllMoveDestinations();
            _commandButtonsVisual.SetActiveButton(CommandType.Move);
        }

        public void OnExit()
        {
            _view.Hide();
        }

        public void OnPointerMove(Vector2 pointerPos)
        {
            if (!_isDragging) return;
            _dragEndScreenPos = pointerPos;

            if (_groundQuery.TryQueryGroundPoint(pointerPos, out var worldPoint))
            {
                _dragEndValidWorldPos = worldPoint;
            }

            UpdatePreviewUI(pointerPos);

            if (Vector2.Distance(_dragStartScreenPos, _dragEndScreenPos) > 10f)
                _movePreviewService.SetFormationDirection(_dragStartWorldPos - _dragEndValidWorldPos);

            _movePreviewService.CalculateSelectionSize(_dragStartWorldPos, _dragEndValidWorldPos);
            RequestPreview();
        }

        private void UpdatePreviewUI(Vector2 pointerPos)
        {
            _view.UpdateSelectionEnd(pointerPos);

            Vector3 linkerDelta = _dragEndScreenPos - _dragStartScreenPos;
            float angle = Vector2.Angle(Vector2.up, linkerDelta);
            bool isNegative = Vector2.Dot(linkerDelta, Vector2.right) > 0;
            _view.SetLinkerZRotation(angle * (isNegative ? -1 : 1));

            Vector2 scaleFactor = _view.GetScreenScaleFactor();

            float finalDeltaY = Vector2.Dot(linkerDelta, Vector2.up) * scaleFactor.y;
            float finalDeltaX = Vector2.Dot(linkerDelta, Vector2.right) * scaleFactor.x;

            float dragLength = new Vector2(finalDeltaY, finalDeltaX).magnitude;
            _view.SetLinkerLength(dragLength);
        }

        public void OnPointerDown(Vector2 pointerPos)
        {
            _dragStartScreenPos = pointerPos;

            if (_groundQuery.TryQueryGroundPoint(pointerPos, out var worldPoint))
            {
                _dragStartWorldPos = worldPoint;
                _dragEndValidWorldPos = worldPoint;
                _isDragging = true;
            }
            else
            {
                _isDragging = false;
                return;
            }

            _view.SetSelectionStart(pointerPos);
            _view.SetLinkerPosition(pointerPos);
            _view.SetLinkerLength(0f);
            _view.UpdateSelectionEnd(pointerPos);
            _view.Show();

            // Application-слой сам достаёт позиции выбранных юнитов через facade
            // и передаёт в доменный сервис готовые данные — сервис ничего не знает про Entity/ECS.
            CollectSelectedUnitsPositions();
            _movePreviewService.CalculateUnitsCenter(_selectedUnitsPositionsBuffer);

            _movePreviewService.SetProjectedFormationCenter(_dragStartWorldPos);

            Vector2 clickFormationDirection =
                (_movePreviewService.ProjectedFormationCenter - _movePreviewService.SelectedUnitsCenter).normalized;
            _movePreviewService.SetFormationDirection(clickFormationDirection);

            _movePreviewService.CalculateSelectionSize(_dragStartWorldPos, _dragEndValidWorldPos);

            RequestPreview();
        }

        public void OnPointerUp(Vector2 pointerPos)
        {
            _isDragging = false;
            _view.Hide();

            Vector2 slotsOrderDirection =
                _movePreviewService.ProjectedFormationCenter - _movePreviewService.SelectedUnitsCenter.normalized;

            _move.CommitMove(slotsOrderDirection);
        }

        private void CollectSelectedUnitsPositions()
        {
            _selectedUnitsPositionsBuffer.Clear();

            var selectedUnits = _unitQuery.GetSelectedUnits();
            for (int i = 0; i < selectedUnits.Count; i++)
            {
                ref var position = ref _unitQuery.GetPosition(selectedUnits[i]);
                _selectedUnitsPositionsBuffer.Add(position.Value);
            }
        }

        private void RequestPreview()
        {
            _move.MovePreviewRequest(
                _movePreviewService.ProjectedFormationCenter,
                _movePreviewService.SelectionSize,
                _movePreviewService.FormationForwardDirection,
                _movePreviewService.FormationRightDirection);
        }
    }
}