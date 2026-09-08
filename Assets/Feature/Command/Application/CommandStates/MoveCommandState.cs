using System.Collections.Generic;
using Feature.Command.Application.CommandStates.Interfaces;
using Feature.Command.Application.Interfaces;
using Feature.Command.Domain;
using Feature.Command.Domain.Data;
using Feature.GameplayECS.Facade.Interfaces;
using Feature.Shared.Math;
using Scellecs.Morpeh;
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
        [Inject] private readonly CommandButtonsVisual _commandButtonsVisual;

        private Formation _formation;
        private Vector2 _dragStartScreenPos;
        private Vector3 _dragStartWorldPos;
        private Vector3 _dragEndValidWorldPos;
        private bool _isDragging;

        public void SetShiftHold(bool shiftHold)
        {
        }

        public void SetCtrlHold(bool ctrlHold)
        {
        }

        public void OnEnter()
        {
            //_move.ShowAllMoveDestinations();
            _commandButtonsVisual.SetActiveButton(CommandType.Move);
        }

        public void OnExit()
        {
            _view.Hide();
            _isDragging = false;
            _move.ClearPreviewRequest();
        }

        public void OnPointerDown(Vector2 pointerPos)
        {
            _dragStartScreenPos = pointerPos;
            
            if (!_groundQuery.TryQueryGroundPoint(pointerPos, out var worldPoint))
            {
                _isDragging = false;
                return;
            }

            _dragStartWorldPos = worldPoint;
            _dragEndValidWorldPos = worldPoint;
            _isDragging = true;

            var unitsCenter = MathExtensions.ToFlat2(AverageWorldPosition(_unitQuery.GetSelectedUnits()));
            _formation = new Formation(unitsCenter, MathExtensions.ToFlat2(worldPoint));

            _view.SetSelectionStart(pointerPos);
            _view.Show();
            _move.NewMovePreviewRequest();
            RequestUpdatePreview();
        }

        public void OnPointerMove(Vector2 pointerPos)
        {
            if (_isDragging == false) return;
            if (_groundQuery.TryQueryGroundPoint(pointerPos, out var worldPoint))
                _dragEndValidWorldPos = worldPoint;

            _view.UpdateDrag(_dragStartScreenPos, pointerPos);
            _formation.UpdateDrag(MathExtensions.ToFlat2(_dragStartWorldPos),
                MathExtensions.ToFlat2(_dragEndValidWorldPos));
            RequestUpdatePreview();
        }

        public void OnPointerUp(Vector2 pointerPos)
        {
            if (_isDragging == false) return;
            
            _isDragging = false;
            _view.Hide();
            Vector3 selectedUnitsCentroid = _unitQuery.GetUnitsCenter(_unitQuery.GetSelectedUnits());
            _move.CommitMove(_formation.GetDirectionToCenter(MathExtensions.ToFlat2(selectedUnitsCentroid)));
        }

        private void RequestUpdatePreview()
            => _move.UpdateMovePreviewRequest(_formation.Center, _formation.Size, _formation.Forward, _formation.Right);

        private Vector3 AverageWorldPosition(IReadOnlyList<Entity> units)
        {
            if (units.Count == 0) return Vector3.zero;
            
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < units.Count; i++)
                sum += _unitQuery.GetPosition(units[i]).Value;
            return sum / units.Count;
        }
    }
}