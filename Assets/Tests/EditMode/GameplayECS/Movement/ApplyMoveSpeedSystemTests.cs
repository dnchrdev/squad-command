using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Movement.Components;
using Features.GameplayECS.Movement.Outputs;
using Features.GameplayECS.Movement.Systems;
using Features.GameplayECS.Navigation.Outputs;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Movement
{
    public class ApplyMoveSpeedSystemTests
    {
        private const float DT = 0.02f;

        private World _world;
        private ApplyMoveSpeedSystem _system;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _system = new ApplyMoveSpeedSystem().Init(_world);
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_AddsSteeringTimesSpeedToDesiredVelocity()
        {
            var unit = CreateMover(Vector3.zero, 3.5f, Vector3.right, true);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(3.5f, 0f, 0f), _world.GetStash<DesiredVelocity>().Get(unit).Value);
        }

        [Test]
        public void OnUpdate_KeepsDesiredVelocityAddedEarlierInStep()
        {
            var unit = CreateMover(new Vector3(1f, 0f, 0f), 2f, Vector3.forward, true);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(1f, 0f, 2f), _world.GetStash<DesiredVelocity>().Get(unit).Value);
        }

        [Test]
        public void OnUpdate_IgnoresEntitiesWithoutUnitTag()
        {
            var entity = CreateMover(Vector3.zero, 3.5f, Vector3.right, false);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(Vector3.zero, _world.GetStash<DesiredVelocity>().Get(entity).Value);
        }

        private Entity CreateMover(Vector3 desiredVelocity, float speed, Vector3 steering, bool isUnit)
        {
            var entity = _world.CreateEntity();
            if (isUnit)
                _world.GetStash<UnitTag>().Set(entity);
            _world.GetStash<DesiredVelocity>().Set(entity, new DesiredVelocity { Value = desiredVelocity });
            _world.GetStash<MovementSpeed>().Set(entity, new MovementSpeed { Value = speed });
            _world.GetStash<SteeringDirection>().Set(entity, new SteeringDirection { Value = steering });
            return entity;
        }
    }
}
