using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Common
{
    public struct UnitTag : IComponent { }
    
    public struct Position : IComponent { public Vector3 Value; }
    public struct Rotation : IComponent { public Quaternion Value; }
}