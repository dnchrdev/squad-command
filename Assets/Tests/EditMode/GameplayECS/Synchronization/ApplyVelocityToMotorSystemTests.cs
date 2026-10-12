using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Synchronization.Components;
using Features.GameplayECS.Synchronization.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.GameplayECS.Synchronization.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Synchronization
{
    public class ApplyVelocityToMotorSystemTests
    {
        private const float DT = 0.02f;

        private World _world;

        [SetUp]
        public void SetUp() => _world = World.Create();

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_SendsUnitVelocityToMotor()
        {
            var system = new ApplyVelocityToMotorSystem().Init(_world);
            var motor = new FakeUnitMotor();
            var unit = _world.CreateEntity();
            _world.GetStash<UnitTag>().Set(unit);
            _world.GetStash<Velocity>().Set(unit, new Velocity { Value = new Vector3(1f, 0f, 2f) });
            _world.GetStash<UnitMotorComponent>().Set(unit, new UnitMotorComponent { Value = motor });
            _world.Commit();

            system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(1f, 0f, 2f), motor.Velocity);
        }
    }
}
