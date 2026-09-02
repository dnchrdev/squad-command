using UnityEngine;

namespace Feature.Command.Application.CommandStates.Interfaces
{
    public interface ICommandState
    {
        void SetShiftHold(bool shiftHold);
        void SetCtrlHold(bool ctrlHold);
        void OnEnter();
        void OnExit();
        void OnPointerDown(Vector2 pointerPos);
        void OnPointerUp(Vector2 pointerPos);
        void OnPointerMove(Vector2 pointerPos);
    }
}