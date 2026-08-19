using UnityEngine;

namespace Feature.Command.Adapter.Interfaces
{
    public interface IGroundQueryService
    {
        bool TryQueryGroundPoint(Vector2 screenPoint, out Vector3 worldPoint);
    }
}