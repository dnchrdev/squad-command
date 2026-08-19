using UnityEngine;

namespace Feature.Command.Adapter.Interfaces
{
    public interface ITargetQueryService
    {
        int? QueryEnemyAtPoint(Vector2 screenPoint);
    }
}