using Scellecs.Morpeh;
using UnityEngine;

namespace Features.Shared.Units
{
    public readonly struct UnitSnapshot
    {
        public readonly Entity Entity;
        public readonly Vector3 Position;

        public UnitSnapshot(Entity entity, Vector3 position)
        {
            Entity = entity;
            Position = position;
        }
    }
}