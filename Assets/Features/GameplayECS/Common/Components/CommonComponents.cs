using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.Common.Components
{
    public struct UnitTag : IComponent { }
    public struct Position : IComponent { public Vector3 Value; }
    public struct Rotation : IComponent { public Quaternion Value; }
    public struct AssetPath : IComponent { public string Value; }
    public struct SpawningTag : IComponent { }
    public struct SelectedTag : IComponent { }
    public struct SteeringDirection : IComponent { public Vector3 Value; }
    public struct Velocity : IComponent { public Vector3 Value; }
}