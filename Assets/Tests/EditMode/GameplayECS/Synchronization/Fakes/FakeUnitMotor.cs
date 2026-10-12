using Features.GameplayECS.Synchronization.View;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Synchronization.Fakes
{
    public sealed class FakeUnitMotor : IUnitMotor
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; } = Quaternion.identity;
        public Vector3 Velocity { get; private set; }

        public void SetPosition(Vector3 position) => Position = position;
        public void SetVelocity(Vector3 velocity) => Velocity = velocity;
    }
}
