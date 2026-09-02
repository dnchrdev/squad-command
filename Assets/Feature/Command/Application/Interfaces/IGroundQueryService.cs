using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Feature.Command.Application.Interfaces
{
    public interface IGroundQueryService
    {
        bool TryQueryGroundPoint(Vector2 screenPoint, out Vector3 worldPoint);
    }
}