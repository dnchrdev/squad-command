using Rect = UnityEngine.Rect;

namespace Feature.Command.Application.Interfaces
{
    public interface ISelectRectView
    {
        public void Show();
        public void Hide();
        public void Initialize();
        public void UpdateSelectionRect(Rect selectionRect);
    }
}