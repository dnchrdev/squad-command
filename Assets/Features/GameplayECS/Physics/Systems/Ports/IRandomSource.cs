using UnityEngine;

namespace Features.GameplayECS.Physics.Systems.Ports
{
    public interface IRandomSource
    {
        Vector3 OnUnitSphere();
    }
}