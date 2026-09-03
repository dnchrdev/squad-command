using Rect = UnityEngine.Rect;

namespace Feature.Command.Application.Interfaces
{
    public interface ISelectRectView
    {
        void Initialize();
        void Show();
        void Hide();
        void UpdateSelectionRect(Rect screenRect);
    }
}