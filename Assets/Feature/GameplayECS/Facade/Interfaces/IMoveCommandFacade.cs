using UnityEngine;

namespace Feature.GameplayECS.Facade.Interfaces
{
    public interface IMoveCommandFacade
    {
        void NewMovePreviewRequest();
        void UpdateMovePreviewRequest(Vector2 center, Vector2 size, Vector2 formationForward, Vector2 formationRight);
        void CommitMove(Vector2 slotsOrderDirection);
        void ClearPreviewRequest();
    }
}