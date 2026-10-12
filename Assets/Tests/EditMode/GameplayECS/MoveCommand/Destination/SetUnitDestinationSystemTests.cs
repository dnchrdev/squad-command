using System.Collections.Generic;
using Features.GameplayECS.Common.Components;
using Features.GameplayECS.MoveCommand.Destination.Components;
using Features.GameplayECS.MoveCommand.Destination.Systems;
using Features.GameplayECS.Navigation.Requests;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.MoveCommand.Destination
{
    public class SetUnitDestinationSystemTests
    {
        private const float DT = 0.016f;

        private static readonly Vector3 SlotPosition = new Vector3(3f, 0f, 4f);

        private World _world;
        private SetUnitDestinationSystem _system;
        private Entity _unit;
        private Entity _formation;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _system = new SetUnitDestinationSystem().Init(_world);

            var slot = _world.CreateEntity();
            _world.GetStash<Position>().Set(slot, new Position { Value = SlotPosition });
            _unit = _world.CreateEntity();
            _world.GetStash<UnitTag>().Set(_unit);
            _world.GetStash<UnitMoveDestinationSlot>().Set(_unit, new UnitMoveDestinationSlot { Slot = slot });

            _formation = _world.CreateEntity();
            _world.GetStash<MoveDestinationFormation>().Set(_formation, new MoveDestinationFormation
            {
                Units = new List<Entity> { _unit },
                Slots = new List<Entity> { slot }
            });
            _world.GetStash<DestinationSlotsAssignedTag>().Set(_formation);
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_ReadyFormation_SendsDestinationRequestToSlot()
        {
            _world.GetStash<ApplyDestinationFormationTag>().Set(_formation);

            Run();

            var request = _world.GetStash<DestinationRequest>().Get(_world.SingleWith<DestinationRequest>());
            Assert.That(request.Target, Is.EqualTo(_unit));
            VectorAssert.AreEqual(SlotPosition, request.Destination);
        }

        [Test]
        public void OnUpdate_ReadyFormation_RemovesApplyTag()
        {
            _world.GetStash<ApplyDestinationFormationTag>().Set(_formation);

            Run();

            Assert.That(_world.GetStash<ApplyDestinationFormationTag>().Has(_formation), Is.False);
        }

        [Test]
        public void OnUpdate_FormationWithoutApplyTag_SendsNothing()
        {
            Run();

            Assert.That(_world.CountWith<DestinationRequest>(), Is.Zero);
        }

        private void Run()
        {
            _world.Commit();
            _system.OnUpdate(DT);
            _world.Commit();
        }
    }
}
