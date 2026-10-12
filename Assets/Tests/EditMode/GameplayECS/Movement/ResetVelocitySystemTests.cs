using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Movement.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Movement
{
    public class ResetVelocitySystemTests
    {
        private const float DT = 0.02f;
        private static readonly Vector3 OldVelocity = new Vector3(1f, 2f, 3f);

        private World _world;
        private ResetVelocitySystem _system;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _system = new ResetVelocitySystem().Init(_world);
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_SetsUnitVelocityToZero()
        {
            var unit = CreateWithVelocity(true);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(Vector3.zero, _world.GetStash<Velocity>().Get(unit).Value);
        }

        [Test]
        public void OnUpdate_IgnoresEntitiesWithoutUnitTag()
        {
            var entity = CreateWithVelocity(false);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(OldVelocity, _world.GetStash<Velocity>().Get(entity).Value);
        }

        private Entity CreateWithVelocity(bool isUnit)
        {
            var entity = _world.CreateEntity();
            if (isUnit)
                _world.GetStash<UnitTag>().Set(entity);
            _world.GetStash<Velocity>().Set(entity, new Velocity { Value = OldVelocity });
            return entity;
        }
    }
}
