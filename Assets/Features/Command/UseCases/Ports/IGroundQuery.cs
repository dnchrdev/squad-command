using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Features.Command.UseCases.Ports
{
    public interface IGroundQuery
    {
        bool TryGetGroundPoint(Vector2 screenPoint, out Vector3 worldPoint);
    }
}