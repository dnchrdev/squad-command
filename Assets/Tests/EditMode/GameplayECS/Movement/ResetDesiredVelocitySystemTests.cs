using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Movement.Outputs;
using Features.GameplayECS.Movement.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Movement
{
    public class ResetDesiredVelocitySystemTests
    {
        private const float DT = 0.02f;
        private static readonly Vector3 OldDesiredVelocity = new Vector3(1f, 2f, 3f);

        private World _world;
        private ResetDesiredVelocitySystem _system;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _system = new ResetDesiredVelocitySystem().Init(_world);
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_SetsUnitDesiredVelocityToZero()
        {
            var unit = CreateWithDesiredVelocity(true);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(Vector3.zero, _world.GetStash<DesiredVelocity>().Get(unit).Value);
        }

        [Test]
        public void OnUpdate_IgnoresEntitiesWithoutUnitTag()
        {
            var entity = CreateWithDesiredVelocity(false);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(OldDesiredVelocity, _world.GetStash<DesiredVelocity>().Get(entity).Value);
        }

        private Entity CreateWithDesiredVelocity(bool isUnit)
        {
            var entity = _world.CreateEntity();
            if (isUnit)
                _world.GetStash<UnitTag>().Set(entity);
            _world.GetStash<DesiredVelocity>().Set(entity, new DesiredVelocity { Value = OldDesiredVelocity });
            return entity;
        }
    }
}
