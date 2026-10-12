using System.Collections.Generic;
using Features.GameplayECS.Common.Components;
using Features.GameplayECS.MoveCommand.Destination.Components;
using Features.GameplayECS.MoveCommand.Destination.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.MoveCommand.Destination
{
    public class AssignDestinationSlotsSystemTests
    {
        private const float DT = 0.016f;

        private World _world;
        private AssignDestinationSlotsSystem _system;
        private Stash<UnitMoveDestinationSlot> _unitSlotStash;
        private Entity _leftUnit;
        private Entity _rightUnit;
        private Entity _rightSlot;
        private Entity _leftSlot;
        private Entity _formation;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _system = new AssignDestinationSlotsSystem().Init(_world);
            _unitSlotStash = _world.GetStash<UnitMoveDestinationSlot>();

            _leftUnit = CreateUnit(new Vector3(0f, 0f, 0f));
            _rightUnit = CreateUnit(new Vector3(10f, 0f, 0f));
            _rightSlot = CreateWithPosition(new Vector3(10f, 0f, 1f));
            _leftSlot = CreateWithPosition(new Vector3(0f, 0f, 1f));

            _formation = _world.CreateEntity();
            _world.GetStash<MoveDestinationFormation>().Set(_formation, new MoveDestinationFormation
            {
                Units = new List<Entity> { _leftUnit, _rightUnit },
                Slots = new List<Entity> { _rightSlot, _leftSlot }
            });
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_AssignsEachUnitItsNearestFreeSlot()
        {
            Run();

            Assert.That(_unitSlotStash.Get(_leftUnit).Slot, Is.EqualTo(_leftSlot));
            Assert.That(_unitSlotStash.Get(_rightUnit).Slot, Is.EqualTo(_rightSlot));
            Assert.That(_unitSlotStash.Get(_leftUnit).OwnedFormation, Is.EqualTo(_formation));
        }

        [Test]
        public void OnUpdate_MarksFormationAssignedAndReadyToApply()
        {
            Run();

            Assert.That(_world.GetStash<DestinationSlotsAssignedTag>().Has(_formation), Is.True);
            Assert.That(_world.GetStash<ApplyDestinationFormationTag>().Has(_formation), Is.True);
        }

        [Test]
        public void OnUpdate_AlreadyAssignedFormation_IsSkipped()
        {
            _world.GetStash<DestinationSlotsAssignedTag>().Set(_formation);

            Run();

            Assert.That(_unitSlotStash.Has(_leftUnit), Is.False);
            Assert.That(_unitSlotStash.Has(_rightUnit), Is.False);
        }

        [Test]
        public void OnUpdate_UnitWithoutPosition_StaysUnassigned()
        {
            _world.GetStash<Position>().Remove(_rightUnit);

            Run();

            Assert.That(_unitSlotStash.Has(_rightUnit), Is.False);
            Assert.That(_unitSlotStash.Get(_leftUnit).Slot, Is.EqualTo(_leftSlot));
        }

        private Entity CreateUnit(Vector3 position)
        {
            var unit = CreateWithPosition(position);
            _world.GetStash<UnitTag>().Set(unit);
            return unit;
        }

        private Entity CreateWithPosition(Vector3 position)
        {
            var entity = _world.CreateEntity();
            _world.GetStash<Position>().Set(entity, new Position { Value = position });
            return entity;
        }

        private void Run()
        {
            _world.Commit();
            _system.OnUpdate(DT);
            _world.Commit();
        }
    }
}
