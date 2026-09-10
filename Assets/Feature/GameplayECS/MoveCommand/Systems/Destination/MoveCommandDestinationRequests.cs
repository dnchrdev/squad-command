using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.Systems.Destination
{
    public struct CommitMoveRequest : IComponent
    {
        public Vector2 OrderDirection;
    }

    public struct ApplyDestinationFormationSelfRequest : IComponent
    {
    }

}