using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Physics
{
    public struct CollisionRadius : IComponent { public float Value; }
    public struct StaticBodyTag : IComponent { }
    public struct PlayerControlledTag : IComponent { }
    public struct AIControlledTag : IComponent { }
    public struct PhysicsForce : IComponent { public Vector3 Value; }
}