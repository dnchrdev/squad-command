using System.Collections.Generic;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.MoveCommand.Destination.Components
{
    public struct UnitMoveDestinationSlot : IComponent
    {
        public Entity Slot;
        public Entity OwnedFormation;
    }

    public struct MoveDestinationSlotTag : IComponent { }

    public struct MoveDestinationFormation : IComponent
    {
        public List<Entity> Slots;
        public List<Entity> Units;
        public Vector2 OrderDirection;
    }

    public struct DestinationSlotsAssignedTag : IComponent { }
    public struct ApplyDestinationFormationTag : IComponent { }
}