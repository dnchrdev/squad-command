using Vector2 = UnityEngine.Vector2;

namespace Features.Command.UseCases.Ports
{
    public interface IMoveDragOutput
    {
        void ShowDrag(Vector2 screenStart);
        void UpdateDrag(Vector2 screenStart, Vector2 screenEnd);
        void HideDrag();
    }
}