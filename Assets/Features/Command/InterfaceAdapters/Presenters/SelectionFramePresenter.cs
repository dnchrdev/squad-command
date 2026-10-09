using Features.Command.InterfaceAdapters.Ports;
using Features.Command.UseCases.Ports;
using Rect = UnityEngine.Rect;

namespace Features.Command.InterfaceAdapters.Presenters
{
    public sealed class SelectionFramePresenter : ISelectionFrameOutput
    {
        private readonly ISelectRectView _view;

        public SelectionFramePresenter(ISelectRectView view)
        {
            _view = view;
        }

        public void ShowFrame(Rect screenRect)
        {
            _view.UpdateSelectionRect(screenRect);
            _view.Show();
        }

        public void UpdateFrame(Rect screenRect) => _view.UpdateSelectionRect(screenRect);

        public void HideFrame() => _view.Hide();
    }
}