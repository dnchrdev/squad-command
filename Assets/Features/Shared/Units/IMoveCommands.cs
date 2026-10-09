using UnityEngine;

namespace Features.Shared.Units
{
    public interface IMoveCommands
    {
        void StartPreview();
        void UpdatePreview(Vector2 center, Vector2 size, Vector2 formationForward, Vector2 formationRight);
        void CommitMove(Vector2 slotsOrderDirection);
        void ClearPreview();
    }
}