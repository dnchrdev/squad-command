using Vector2 = UnityEngine.Vector2;

namespace Feature.Command.Application.Interfaces
{
    public interface ITargetQueryService
    {
        int? QueryEnemyAtPoint(Vector2 screenPoint);
    }
}