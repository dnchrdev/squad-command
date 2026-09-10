using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.Adapter
{
    public interface IMoveSlotView
    {
        public void Show();
        public void Hide();
        public void SetPosition(Vector3 position);
    }
}