using Rect = UnityEngine.Rect;

namespace Features.Command.InterfaceAdapters.Ports
{
    public interface ISelectRectView
    {
        void Initialize();
        void Show();
        void Hide();
        void UpdateSelectionRect(Rect screenRect);
    }
}