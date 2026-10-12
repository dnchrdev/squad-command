using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Movement.Outputs;
using Features.GameplayECS.Physics.Components;
using Features.GameplayECS.Physics.Outputs;
using Features.GameplayECS.Physics.Systems;
using Features.GameplayECS.Synchronization.Outputs;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.GameplayECS.Physics.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Physics
{
    public class ResolveCollisionsSystemTests
    {
        private const float DT = 0.02f;
        private const float RADIUS = 0.5f;

        private static readonly Vector3 Origin = Vector3.zero;
        private static readonly Vector3 Overlapping = new Vector3(0.8f, 0f, 0f);
        private static readonly Vector3 FarAway = new Vector3(5f, 0f, 0f);

        private World _world;
        private StubRandomSource _random;
        private ResolveCollisionsSystem _system;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _random = new StubRandomSource();
            _system = new ResolveCollisionsSystem(_random).Init(_world);
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_NoOverlap_KeepsOwnVelocity()
        {
            var unit = CreatePlayerUnit(Origin, new Vector3(1f, 0f, 0f));
            CreatePlayerUnit(FarAway, Vector3.zero);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(1f, 0f, 0f), VelocityOf(unit));
        }

        [Test]
        public void OnUpdate_MovingIntoStaticBody_StopsAndPushesOut()
        {
            var unit = CreatePlayerUnit(Origin, new Vector3(2f, 0f, 0f));
            CreateStaticBody(Overlapping);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(-1.25f, 0f, 0f), VelocityOf(unit));
        }

        [Test]
        public void OnUpdate_MovingAwayFromStaticBody_KeepsVelocity()
        {
            var unit = CreatePlayerUnit(Origin, new Vector3(-2f, 0f, 0f));
            CreateStaticBody(Overlapping);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(-2f, 0f, 0f), VelocityOf(unit));
        }

        [Test]
        public void OnUpdate_MovingIntoEnemy_TreatsEnemyAsObstacle()
        {
            var unit = CreatePlayerUnit(Origin, new Vector3(2f, 0f, 0f));
            var enemy = CreateBody(Overlapping, new Vector3(-5f, 0f, 0f));
            _world.GetStash<AIControlledTag>().Set(enemy);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(-1.25f, 0f, 0f), VelocityOf(unit));
        }

        [Test]
        public void OnUpdate_AllyMovingIntoUnit_PushesItAlong()
        {
            var unit = CreatePlayerUnit(Origin, Vector3.zero);
            CreatePlayerUnit(Overlapping, new Vector3(-2f, 0f, 0f));
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(-2f, 0f, 0f), VelocityOf(unit));
        }

        [Test]
        public void OnUpdate_OverlappingStillUnits_PushApartInverselyToDistance()
        {
            var unit = CreatePlayerUnit(Origin, Vector3.zero);
            var other = CreatePlayerUnit(Overlapping, Vector3.zero);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(-1.25f, 0f, 0f), VelocityOf(unit));
            VectorAssert.AreEqual(new Vector3(1.25f, 0f, 0f), VelocityOf(other));
        }

        [Test]
        public void OnUpdate_UnitsAtSamePoint_PushInRandomFlatDirection()
        {
            _random.Value = new Vector3(0.6f, 0.5f, 0.8f);
            var unit = CreatePlayerUnit(Origin, Vector3.zero);
            CreatePlayerUnit(Origin, Vector3.zero);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(60f, 0f, 80f), VelocityOf(unit));
        }

        [Test]
        public void OnUpdate_LeavesDesiredVelocityUntouched()
        {
            var unit = CreatePlayerUnit(Origin, new Vector3(2f, 0f, 0f));
            CreateStaticBody(Overlapping);
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(2f, 0f, 0f), _world.GetStash<DesiredVelocity>().Get(unit).Value);
        }

        private Entity CreatePlayerUnit(Vector3 position, Vector3 force)
        {
            var unit = CreateBody(position, force);
            _world.GetStash<PlayerControlledTag>().Set(unit);
            return unit;
        }

        private Entity CreateBody(Vector3 position, Vector3 force)
        {
            var body = CreateCollider(position);
            _world.GetStash<DesiredVelocity>().Set(body, new DesiredVelocity { Value = force });
            _world.GetStash<Velocity>().Set(body, new Velocity { Value = Vector3.zero });
            return body;
        }

        private void CreateStaticBody(Vector3 position)
        {
            var body = CreateCollider(position);
            _world.GetStash<StaticBodyTag>().Set(body);
        }

        private Entity CreateCollider(Vector3 position)
        {
            var body = _world.CreateEntity();
            _world.GetStash<UnitTag>().Set(body);
            _world.GetStash<Position>().Set(body, new Position { Value = position });
            _world.GetStash<CollisionRadius>().Set(body, new CollisionRadius { Value = RADIUS });
            return body;
        }

        private Vector3 VelocityOf(Entity entity) => _world.GetStash<Velocity>().Get(entity).Value;
    }
}
