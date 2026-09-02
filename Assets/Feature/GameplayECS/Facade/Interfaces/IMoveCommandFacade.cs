using UnityEngine;

namespace Feature.GameplayECS.Facade.Interfaces
{
    public interface IMoveCommandFacade
    {
        void ShowAllMoveDestinations();
        void StopAll();
        void ClearSelected();
        void MovePreviewRequest(Vector2 center, Vector2 size, Vector2 formationForward, Vector2 formationRight);
        void CommitMove(Vector2 slotsOrderDirection);
    }
}