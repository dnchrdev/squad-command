using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.Systems.Preview
{
    public struct HideMovePreviewSlotsRequest : IComponent
    {
    }

    public struct NewMovePreviewFormationRequest : IComponent
    {
    }

    public struct UpdateMovePreviewFormationRequest : IComponent
    {
        public Vector2 Center;
        public Vector2 Size;
        public Vector2 FormationForward;
        public Vector2 FormationRight;
    }
    

}