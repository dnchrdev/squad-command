using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand
{
    public struct MovePreviewRequest : IComponent
    {
        public Vector2 Center;
        public Vector2 Size;
        public Vector2 FormationForward;
        public Vector2 FormationRight;
    }

    public struct HideMovePreviewRequest : IComponent
    {
    }
    
    public struct CreatePreviewSlotRequest : IComponent
    {
        public int SlotIndex;
        public Vector3 Position;
    }
        
    public struct CommitMoveRequest : IComponent
    {
        public Vector2 FormationForward;
    }
    
}