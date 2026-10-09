using Rect = UnityEngine.Rect;

namespace Features.Command.UseCases.Ports
{
    public interface ISelectionFrameOutput
    {
        void ShowFrame(Rect screenRect);
        void UpdateFrame(Rect screenRect);
        void HideFrame();
    }
}