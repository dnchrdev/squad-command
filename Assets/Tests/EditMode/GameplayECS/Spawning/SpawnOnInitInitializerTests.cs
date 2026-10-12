using System.Collections.Generic;
using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Movement.Components;
using Features.GameplayECS.Movement.Outputs;
using Features.GameplayECS.Navigation.Outputs;
using Features.GameplayECS.Physics.Components;
using Features.GameplayECS.Physics.Outputs;
using Features.GameplayECS.Spawning.Systems;
using Features.GameplayECS.Synchronization.Outputs;
using Features.GameplayECS.UnitView.Components;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Spawning
{
    public class SpawnOnInitInitializerTests
    {
        private const int GRID_SIDE = 7;
        private const float SPAWN_HEIGHT = 1f;

        private World _world;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            new SpawnOnInitInitializer().Init(_world);
            _world.Commit();
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnAwake_CreatesUnitPerGridCell()
        {
            Assert.That(_world.CountWith<UnitTag>(), Is.EqualTo(GRID_SIDE * GRID_SIDE));
        }

        [Test]
        public void OnAwake_UnitsHaveEverythingGameplayNeeds()
        {
            var filter = _world.Filter
                .With<UnitTag>()
                .With<MovementSpeed>()
                .With<AssetPath>()
                .With<Position>()
                .With<Rotation>()
                .With<Velocity>()
                .With<SteeringDirection>()
                .With<DesiredVelocity>()
                .With<CollisionRadius>()
                .With<PlayerControlledTag>()
                .Build();

            var count = 0;
            foreach (var _ in filter)
                count++;

            Assert.That(count, Is.EqualTo(GRID_SIDE * GRID_SIDE));
        }

        [Test]
        public void OnAwake_PlacesUnitsOnGridAboveGround()
        {
            var expected = new List<Vector3>();
            for (int x = 0; x < GRID_SIDE; x++)
            for (int z = 0; z < GRID_SIDE; z++)
                expected.Add(new Vector3(x, SPAWN_HEIGHT, z));

            var actual = new List<Vector3>();
            var positions = _world.GetStash<Position>();
            foreach (var unit in _world.Filter.With<UnitTag>().Build())
                actual.Add(positions.Get(unit).Value);

            Assert.That(actual, Is.EquivalentTo(expected));
        }
    }
}
