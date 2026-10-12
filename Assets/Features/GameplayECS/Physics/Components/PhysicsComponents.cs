using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.Physics.Components
{
    public struct CollisionRadius : IComponent { public float Value; }
    public struct StaticBodyTag : IComponent { }
    public struct PlayerControlledTag : IComponent { }
    public struct AIControlledTag : IComponent { }
}