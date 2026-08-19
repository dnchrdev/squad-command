using Feature.Command.Adapter.Interfaces;
using Feature.Command.Application.CursorModes.Interfaces;
using UnityEngine;
using Zenject;

namespace Feature.Command.Application.CursorModes
{
    public sealed class MoveCursorMode : ICursorMode
    {
        [Inject] private readonly IGroundQueryService _groundQuery;
        [Inject] private readonly MoveSquadUseCase _moveSquadUseCase;

        public void OnEnter()
        {
        }

        public void OnExit()
        {
        }

        public void OnPointerMove(Vector2 screenPos)
        {
        }

        public void OnPointerDown(Vector2 screenPos)
        {
            if (_groundQuery.TryQueryGroundPoint(screenPos, out var worldPoint))
                _moveSquadUseCase.MoveSelectedTo(worldPoint);
        }

        public void OnPointerUp(Vector2 screenPos)
        {
        }
    }
}