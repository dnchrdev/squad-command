using UnityEngine;

namespace Feature.Command.Infrastructure.Interfaces
{
    public interface ISelectRectView
    {
        public void Show();
        public void Hide();
        public void UpdateSelectionRect(Rect selectionRect);
    }
}