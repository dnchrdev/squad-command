using Feature.Command.Application.CommandModes.Interfaces;
using Feature.Command.Application.Interfaces;
using Feature.Command.Domain.Data;
using Feature.GameplayECS.Facade.Interfaces;
using UnityEngine;
using Zenject;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Feature.Command.Application.CommandModes
{
    public sealed class MoveCommandState : ICommandState
    {
        [Inject] private readonly IUnitQueryFacade  _unitQuery;
        [Inject] private readonly IMoveVisualView _view;
        [Inject] private readonly IGroundQueryService _groundQuery;
        [Inject] private readonly IMoveCommandFacade _move;
        [Inject] private readonly MovePreviewService _movePreviewService;
        [Inject] private readonly CommandButtonVisual _commandButtonVisual;
        

        private Vector2 _dragStartScreenPos;
        private Vector2 _dragEndScreenPos;
        private Vector3 _dragStartWorldPos;
        private Vector3 _dragEndValidWorldPos;
        private bool _isDragging;
        private bool _shiftHold;

        public void SetShiftHold(bool shiftHold)
        {
            _shiftHold = shiftHold;
        }

        public void SetCtrlHold(bool ctrlHold)
        {
        }

        public void OnEnter()
        {
            _move.ShowAllMoveDestinations();
            _commandButtonVisual.SetActiveButton(CommandType.Move);
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
            
                    //Debug.Log($"FormationForwardDirection = {_movePreviewService.FormationForwardDirection}");
            
            _movePreviewService.CalculateSelectionSize(_dragStartWorldPos, _dragEndValidWorldPos);
            _move.MovePreviewRequest(
                _movePreviewService.ProjectedFormationCenter,
                _movePreviewService.SelectionSize,
                _movePreviewService.FormationForwardDirection,
                _movePreviewService.FormationRightDirection);
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
            
            _movePreviewService.CalculateSelectedUnitsCenter();
            _movePreviewService.SetProjectedFormationCenter(_dragStartWorldPos);
            
            Vector2 clickFormationDirection = (_movePreviewService.ProjectedFormationCenter - _movePreviewService.SelectedUnitsCenter).normalized;
            _movePreviewService.SetFormationDirection(clickFormationDirection);
            
            _movePreviewService.CalculateSelectionSize(_dragStartWorldPos, _dragEndValidWorldPos);
            
            _move.MovePreviewRequest(
                _movePreviewService.ProjectedFormationCenter,
                _movePreviewService.SelectionSize,
                _movePreviewService.FormationForwardDirection,
                _movePreviewService.FormationRightDirection);
        }

        public void OnPointerUp(Vector2 pointerPos)
        {
            _isDragging = false;

            _view.Hide();
            //
            // _moveUnitsService.CalculateMoveData(_dragStartWorldPos, _dragEndValidWorldPos);
            // _move.MovePreviewRequest(
            //     _moveUnitsService.ProjectedCenter,
            //     _moveUnitsService.SelectionSize,
            //     _moveUnitsService.FormationForwardDirection,
            //     _moveUnitsService.FormationRightDirection);

            //_movePreviewService.SetMoveDirectionStartPoint(_dragEndValidWorldPos);
            
            Vector2 slotsOrderDirection = _movePreviewService.ProjectedFormationCenter - _movePreviewService.SelectedUnitsCenter.normalized;
            
            _move.CommitMove(slotsOrderDirection);
        }
    }
}