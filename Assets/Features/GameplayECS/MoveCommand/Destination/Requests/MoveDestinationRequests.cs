using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.MoveCommand.Destination.Requests
{
    public struct CommitMoveRequest : IComponent
    {
        public Vector2 OrderDirection;
    }
}