using UnityEngine;

namespace Feature.Command.Application.Interfaces
{
    public interface ITargetQueryService
    {
        int? QueryEnemyAtPoint(Vector2 screenPoint);
    }
}