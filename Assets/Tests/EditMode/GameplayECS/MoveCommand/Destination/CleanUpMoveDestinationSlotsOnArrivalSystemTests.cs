using System.Collections.Generic;
using Features.GameplayECS.Common.Components;
using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Destination.Components;
using Features.GameplayECS.MoveCommand.Destination.Systems;
using Features.GameplayECS.MoveCommand.View;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.GameplayECS.MoveCommand.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.MoveCommand.Destination
{
    public class CleanUpMoveDestinationSlotsOnArrivalSystemTests
    {
        private const float DT = 0.016f;

        private World _world;
        private FakeObjectPool<IMoveSlotView> _pool;
        private CleanUpMoveDestinationSlotsOnArrivalSystem _system;
        private SpyMoveSlotView _view;
        private Entity _unit;
        private Entity _slot;
        private Entity _formation;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _pool = new FakeObjectPool<IMoveSlotView>();
            _system = new CleanUpMoveDestinationSlotsOnArrivalSystem(_pool).Init(_world);

            _view = new SpyMoveSlotView();
            _slot = _world.CreateEntity();
            _world.GetStash<Position>().Set(_slot, new Position { Value = Vector3.zero });
            _world.GetStash<MoveDestinationSlotTag>().Set(_slot);
            _world.GetStash<MoveSlotViewComponent>().Set(_slot, new MoveSlotViewComponent { Value = _view });
            _world.GetStash<MoveSlotViewShowedTag>().Set(_slot);

            _formation = _world.CreateEntity();
            _unit = _world.CreateEntity();
            _world.GetStash<UnitMoveDestinationSlot>().Set(_unit,
                new UnitMoveDestinationSlot { Slot = _slot, OwnedFormation = _formation });
            _world.GetStash<MoveDestinationFormation>().Set(_formation, new MoveDestinationFormation
            {
                Units = new List<Entity> { _unit },
                Slots = new List<Entity> { _slot }
            });
            _world.GetStash<DestinationSlotsAssignedTag>().Set(_formation);
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_UnitAtSlotEdge_HidesAndReturnsView()
        {
            PlaceUnit(new Vector3(0.25f, 5f, 0f));

            Run();

            Assert.That(_view.IsVisible, Is.False);
            Assert.That(_pool.Returned, Is.EqualTo(new[] { _view }));
        }

        [Test]
        public void OnUpdate_UnitAtSlot_RemovesSlotAndReleasesUnit()
        {
            PlaceUnit(Vector3.zero);

            Run();

            var formation = _world.GetStash<MoveDestinationFormation>().Get(_formation);
            Assert.That(formation.Slots, Is.Empty);
            Assert.That(formation.Units, Is.Empty);
            Assert.That(_world.GetStash<UnitMoveDestinationSlot>().Has(_unit), Is.False);
            Assert.That(_world.CountWith<MoveDestinationSlotTag>(), Is.Zero);
        }

        [Test]
        public void OnUpdate_UnitFarFromSlot_KeepsSlot()
        {
            PlaceUnit(new Vector3(1f, 0f, 0f));

            Run();

            Assert.That(_view.IsVisible, Is.True);
            Assert.That(_world.GetStash<MoveDestinationFormation>().Get(_formation).Slots, Has.Count.EqualTo(1));
        }

        [Test]
        public void OnUpdate_FormationStillApplying_IsSkipped()
        {
            _world.GetStash<ApplyDestinationFormationTag>().Set(_formation);
            PlaceUnit(Vector3.zero);

            Run();

            Assert.That(_view.IsVisible, Is.True);
            Assert.That(_world.GetStash<UnitMoveDestinationSlot>().Has(_unit), Is.True);
        }

        private void PlaceUnit(Vector3 position)
        {
            _world.GetStash<Position>().Set(_unit, new Position { Value = position });
        }

        private void Run()
        {
            _world.Commit();
            _system.OnUpdate(DT);
            _world.Commit();
        }
    }
}
