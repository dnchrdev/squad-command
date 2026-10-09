using Features.Command.InterfaceAdapters.Ports;
using Features.Command.UseCases.Ports;
using Vector2 = UnityEngine.Vector2;

namespace Features.Command.InterfaceAdapters.Presenters
{
    public sealed class MoveDragPresenter : IMoveDragOutput
    {
        private readonly IMoveVisualView _view;

        public MoveDragPresenter(IMoveVisualView view)
        {
            _view = view;
        }

        public void ShowDrag(Vector2 screenStart)
        {
            _view.SetSelectionStart(screenStart);
            _view.Show();
        }

        public void UpdateDrag(Vector2 screenStart, Vector2 screenEnd) => _view.UpdateDrag(screenStart, screenEnd);

        public void HideDrag() => _view.Hide();
    }
}