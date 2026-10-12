using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Navigation.Components;
using Features.GameplayECS.Navigation.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Navigation
{
    public class CalculateNavigationDirectionSystemTests
    {
        private const float DT = 0.02f;
        private const int FIRST_CORNER = 1;

        private World _world;
        private CalculateNavigationDirectionSystem _system;
        private Stash<Destination> _destinationStash;
        private Stash<SteeringDirection> _steeringStash;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _system = new CalculateNavigationDirectionSystem().Init(_world);
            _destinationStash = _world.GetStash<Destination>();
            _steeringStash = _world.GetStash<SteeringDirection>();
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_FarFromFinalCorner_SteersTowardItOnGroundPlane()
        {
            var unit = CreateUnit(Vector3.zero, WithPath(Vector3.zero, new Vector3(0f, 5f, 10f)));
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(Vector3.forward, _steeringStash.Get(unit).Value);
        }

        [Test]
        public void OnUpdate_NearIntermediateCorner_SteersToNextCorner()
        {
            var unit = CreateUnit(Vector3.zero,
                WithPath(Vector3.zero, new Vector3(0.5f, 0f, 0f), new Vector3(0.5f, 0f, 10f)));
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_destinationStash.Get(unit).CurrentCornerIndex, Is.EqualTo(2));
            VectorAssert.AreEqual(new Vector3(0.0499376f, 0f, 0.9987523f), _steeringStash.Get(unit).Value);
        }

        [Test]
        public void OnUpdate_WithinFinalTolerance_StopsAndRemovesDestination()
        {
            var unit = CreateUnit(new Vector3(0f, 0f, 9.95f), WithPath(Vector3.zero, new Vector3(0f, 0f, 10f)));
            _steeringStash.Set(unit, new SteeringDirection { Value = Vector3.forward });
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(Vector3.zero, _steeringStash.Get(unit).Value);
            Assert.That(_destinationStash.Has(unit), Is.False);
        }

        [Test]
        public void OnUpdate_NoPath_StopsAndRemovesDestination()
        {
            var unit = CreateUnit(Vector3.zero, new Destination { HasPath = false });
            _steeringStash.Set(unit, new SteeringDirection { Value = Vector3.right });
            _world.Commit();

            _system.OnUpdate(DT);

            VectorAssert.AreEqual(Vector3.zero, _steeringStash.Get(unit).Value);
            Assert.That(_destinationStash.Has(unit), Is.False);
        }

        [Test]
        public void OnUpdate_StopWithoutSteering_DoesNotAddSteering()
        {
            var unit = CreateUnit(Vector3.zero, new Destination { HasPath = false });
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_steeringStash.Has(unit), Is.False);
        }

        [Test]
        public void OnUpdate_IgnoresEntitiesWithoutUnitTag()
        {
            var entity = _world.CreateEntity();
            _world.GetStash<Position>().Set(entity, new Position { Value = Vector3.zero });
            _destinationStash.Set(entity, WithPath(Vector3.zero, new Vector3(0f, 0f, 10f)));
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_steeringStash.Has(entity), Is.False);
        }

        private Entity CreateUnit(Vector3 position, Destination destination)
        {
            var unit = _world.CreateEntity();
            _world.GetStash<UnitTag>().Set(unit);
            _world.GetStash<Position>().Set(unit, new Position { Value = position });
            _destinationStash.Set(unit, destination);
            return unit;
        }

        private static Destination WithPath(params Vector3[] corners) => new Destination
        {
            Corners = corners,
            CurrentCornerIndex = FIRST_CORNER,
            HasPath = true,
            Target = corners[corners.Length - 1]
        };
    }
}
