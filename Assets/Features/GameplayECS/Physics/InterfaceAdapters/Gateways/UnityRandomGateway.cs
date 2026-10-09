using Features.GameplayECS.Physics.Systems.Ports;
using UnityEngine;

namespace Features.GameplayECS.Physics.InterfaceAdapters.Gateways
{
    public sealed class UnityRandomGateway : IRandomSource
    {
        public Vector3 OnUnitSphere() => UnityEngine.Random.onUnitSphere;
    }
}