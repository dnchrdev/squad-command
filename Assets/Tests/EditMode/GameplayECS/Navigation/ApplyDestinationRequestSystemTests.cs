using Features.GameplayECS.Navigation.Components;
using Features.GameplayECS.Navigation.Requests;
using Features.GameplayECS.Navigation.Systems;
using Features.GameplayECS.Synchronization.Outputs;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.GameplayECS.Navigation.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.Navigation
{
    public class ApplyDestinationRequestSystemTests
    {
        private static readonly Vector3 UnitPosition = new Vector3(1f, 0f, 2f);
        private static readonly Vector3 RequestedPoint = new Vector3(5f, 0f, 0f);

        private World _world;
        private FakePathService _pathService;
        private ApplyDestinationRequestSystem _system;
        private Entity _unit;
        private Entity _request;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _pathService = new FakePathService();
            _system = new ApplyDestinationRequestSystem(_pathService) { World = _world };
            _system.OnAwake();

            _unit = _world.CreateEntity();
            _world.GetStash<Position>().Set(_unit, new Position { Value = UnitPosition });

            _request = _world.CreateEntity();
            _world.GetStash<DestinationRequest>().Set(_request,
                new DestinationRequest { Target = _unit, Destination = RequestedPoint });
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_PassesUnitPositionAndRequestedPointToPathService()
        {
            _world.Commit();

            _system.OnUpdate(0f);

            VectorAssert.AreEqual(UnitPosition, _pathService.LastFrom);
            VectorAssert.AreEqual(RequestedPoint, _pathService.LastRequested);
        }

        [Test]
        public void OnUpdate_PathFound_SetsDestinationFromSecondCorner()
        {
            var corners = new[] { UnitPosition, RequestedPoint };
            _pathService.Result = true;
            _pathService.Target = RequestedPoint;
            _pathService.Corners = corners;
            _world.Commit();

            _system.OnUpdate(0f);

            var destination = _world.GetStash<Destination>().Get(_unit);
            Assert.That(destination.HasPath, Is.True);
            Assert.That(destination.CurrentCornerIndex, Is.EqualTo(1));
            Assert.That(destination.Corners, Is.SameAs(corners));
            VectorAssert.AreEqual(RequestedPoint, destination.Target);
        }

        [Test]
        public void OnUpdate_NoPath_SetsDestinationWithoutPath()
        {
            _pathService.Result = false;
            _world.Commit();

            _system.OnUpdate(0f);

            var destination = _world.GetStash<Destination>().Get(_unit);
            Assert.That(destination.HasPath, Is.False);
            Assert.That(destination.Corners, Is.Null);
        }

        [Test]
        public void OnUpdate_ConsumesRequest()
        {
            _world.Commit();

            _system.OnUpdate(0f);

            Assert.That(_world.GetStash<DestinationRequest>().Has(_request), Is.False);
        }
    }
}