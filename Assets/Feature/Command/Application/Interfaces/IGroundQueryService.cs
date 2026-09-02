using UnityEngine;

namespace Feature.Command.Application.Interfaces
{
    public interface IGroundQueryService
    {
        bool TryQueryGroundPoint(Vector2 screenPoint, out Vector3 worldPoint);
    }
}