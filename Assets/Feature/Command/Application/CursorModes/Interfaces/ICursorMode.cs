using UnityEngine;

namespace Feature.Command.Application.CursorModes.Interfaces
{
    public interface ICursorMode
    {
        void OnEnter();
        void OnExit();
        void OnPointerDown(Vector2 screenPos);
        void OnPointerUp(Vector2 screenPos);
        void OnPointerMove(Vector2 screenPos);
    }
}