using UnityEngine;

namespace Feature.Command.Application.Interfaces
{
    public interface ISelectRectView
    {
        public void Show();
        public void Hide();
        public void UpdateSelectionRect(Rect selectionRect);
    }
}