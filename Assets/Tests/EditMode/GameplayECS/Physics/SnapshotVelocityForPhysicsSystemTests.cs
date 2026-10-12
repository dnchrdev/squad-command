using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Physics.Components;
using Features.GameplayECS.Physics.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Physics
{
    public class SnapshotVelocityForPhysicsSystemTests
    {
        private const float DT = 0.02f;

        private World _world;

        [SetUp]
        public void SetUp() => _world = World.Create();

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_CopiesVelocityIntoPhysicsForce()
        {
            var system = new SnapshotVelocityForPhysicsSystem().Init(_world);
            var unit = _world.CreateEntity();
            _world.GetStash<UnitTag>().Set(unit);
            _world.GetStash<Velocity>().Set(unit, new Velocity { Value = new Vector3(1f, 2f, 3f) });
            _world.GetStash<PhysicsForce>().Set(unit, new PhysicsForce { Value = Vector3.zero });
            _world.Commit();

            system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(1f, 2f, 3f), _world.GetStash<PhysicsForce>().Get(unit).Value);
        }
    }
}
