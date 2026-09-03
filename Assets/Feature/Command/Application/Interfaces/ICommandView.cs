using Feature.Command.Domain.Data;
using R3;

namespace Feature.Command.Application.Interfaces
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