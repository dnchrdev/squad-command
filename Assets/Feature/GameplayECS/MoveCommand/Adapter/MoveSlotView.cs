using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.Adapter
{
    public class MoveSlotView : MonoBehaviour, IMoveSlotView
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
    }
}