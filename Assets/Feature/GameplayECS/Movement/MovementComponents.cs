using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.AI;

namespace Feature.GameplayECS.Movement
{
    public struct MovementSpeed : IComponent { public float Value; }
    public struct Velocity : IComponent { public Vector3 Value; }
    public struct UnitMotorComponent : IComponent { public IUnitMotor Value; }
}