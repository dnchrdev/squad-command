using Features.GameplayECS.MoveCommand.View;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.MoveCommand.Fakes
{
    public sealed class SpyMoveSlotView : IMoveSlotView
    {
        public bool IsVisible = true;
        public int ShowCalls;
        public int HideCalls;
        public Vector3 Position;

        public void Show()
        {
            IsVisible = true;
            ShowCalls++;
        }

        public void Hide()
        {
            IsVisible = false;
            HideCalls++;
        }

        public void SetPosition(Vector3 position) => Position = position;
    }
}
