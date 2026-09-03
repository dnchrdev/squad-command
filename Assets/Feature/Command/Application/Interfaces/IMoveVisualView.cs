using Vector2 = UnityEngine.Vector2;

namespace Feature.Command.Application.Interfaces
{
    public interface IMoveVisualView
    {
        void Initialize();
        void Show();
        void Hide();
        void SetSelectionStart(Vector2 screenPos);
        void UpdateDrag(Vector2 screenStart, Vector2 screenEnd);
    }
}