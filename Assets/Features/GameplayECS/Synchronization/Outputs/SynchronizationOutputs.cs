using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.Synchronization.Outputs
{
    public struct Position : IComponent { public Vector3 Value; }
    public struct Rotation : IComponent { public Quaternion Value; }
}
