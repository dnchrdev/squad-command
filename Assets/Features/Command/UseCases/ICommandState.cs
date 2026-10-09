using Vector2 = UnityEngine.Vector2;

namespace Features.Command.UseCases
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
        void Canceled();
    }
}