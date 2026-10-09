using UnityEngine;

namespace Features.GameplayECS.MoveCommand.View
{
    public interface IMoveSlotView
    {
        public void Show();
        public void Hide();
        public void SetPosition(Vector3 position);
    }
}