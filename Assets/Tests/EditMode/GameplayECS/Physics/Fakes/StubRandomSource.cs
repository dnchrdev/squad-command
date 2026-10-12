using Features.GameplayECS.Physics.Systems.Ports;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Physics.Fakes
{
    public sealed class StubRandomSource : IRandomSource
    {
        public Vector3 Value;

        public Vector3 OnUnitSphere() => Value;
    }
}
