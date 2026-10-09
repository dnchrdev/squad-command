using Features.Command.Entities;
using R3;

namespace Features.Command.InterfaceAdapters.Ports
{
    public interface ICommandView
    {
        void Initialize();
        Observable<Unit> GetClick(CommandType type);
        float GetLowHeight();
        float GetHighHeight();
        float GetButtonHeight(CommandType type);
        void SetButtonHeightOffset(CommandType type, float height);
    }
}