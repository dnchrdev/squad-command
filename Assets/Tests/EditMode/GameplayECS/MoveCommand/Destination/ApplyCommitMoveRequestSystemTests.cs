using System.Collections.Generic;
using Features.GameplayECS.Common.Components;
using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Destination.Components;
using Features.GameplayECS.MoveCommand.Destination.Requests;
using Features.GameplayECS.MoveCommand.Destination.Systems;
using Features.GameplayECS.MoveCommand.Preview.Requests;
using Features.GameplayECS.Selection.Outputs;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.MoveCommand.Destination
{
    public class ApplyCommitMoveRequestSystemTests
    {
        private const float DT = 0.016f;
        private const string DESTINATION_SLOT_ASSET_PATH = "MoveSlotDestination";

        private static readonly Vector2 OrderDirection = new Vector2(0f, 1f);

        private World _world;
        private ApplyCommitMoveRequestSystem _system;
        private Stash<MoveDestinationFormation> _formationStash;
        private Stash<MoveSlotPosition> _positionStash;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _system = new ApplyCommitMoveRequestSystem().Init(_world);
            _formationStash = _world.GetStash<MoveDestinationFormation>();
            _positionStash = _world.GetStash<MoveSlotPosition>();
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_CreatesFormationForSelectedUnits()
        {
            var first = CreateSelectedUnit();
            var second = CreateSelectedUnit();
            CreatePreviewSlot(new Vector3(1f, 0f, 1f));
            CreatePreviewSlot(new Vector3(2f, 0f, 2f));
            SendRequest();

            Run();

            var formation = _formationStash.Get(_world.SingleWith<MoveDestinationFormation>());
            Assert.That(formation.Units, Is.EquivalentTo(new[] { first, second }));
            Assert.That(formation.Slots, Has.Count.EqualTo(2));
            VectorAssert.AreEqual(OrderDirection, formation.OrderDirection);
        }

        [Test]
        public void OnUpdate_NewDestinationSlotsTakePreviewPositions()
        {
            CreateSelectedUnit();
            CreateSelectedUnit();
            CreatePreviewSlot(new Vector3(1f, 0f, 1f));
            CreatePreviewSlot(new Vector3(2f, 0f, 2f));
            SendRequest();

            Run();

            var slots = _formationStash.Get(_world.SingleWith<MoveDestinationFormation>()).Slots;
            var positions = new List<Vector3>();
            foreach (var slot in slots)
            {
                positions.Add(_positionStash.Get(slot).Value);
                Assert.That(_world.GetStash<MoveDestinationSlotTag>().Has(slot), Is.True);
                Assert.That(_world.GetStash<MoveSlotAssetPath>().Get(slot).Value, Is.EqualTo(DESTINATION_SLOT_ASSET_PATH));
            }
            Assert.That(positions, Is.EquivalentTo(new[] { new Vector3(1f, 0f, 1f), new Vector3(2f, 0f, 2f) }));
        }

        [Test]
        public void OnUpdate_ConsumesRequestAndAsksToHidePreview()
        {
            CreateSelectedUnit();
            CreatePreviewSlot(Vector3.zero);
            SendRequest();

            Run();

            Assert.That(_world.CountWith<CommitMoveRequest>(), Is.Zero);
            Assert.That(_world.CountWith<HideMovePreviewSlotsRequest>(), Is.EqualTo(1));
        }

        [Test]
        public void OnUpdate_UnitAlreadyMoving_ReusesItsSlotAndLeavesOldFormation()
        {
            var unit = CreateSelectedUnit();
            var oldSlot = _world.CreateEntity();
            _positionStash.Set(oldSlot, new MoveSlotPosition { Value = Vector3.zero });
            var oldFormation = _world.CreateEntity();
            _formationStash.Set(oldFormation, new MoveDestinationFormation
            {
                Slots = new List<Entity> { oldSlot },
                Units = new List<Entity> { unit }
            });
            _world.GetStash<UnitMoveDestinationSlot>().Set(unit,
                new UnitMoveDestinationSlot { Slot = oldSlot, OwnedFormation = oldFormation });
            CreatePreviewSlot(new Vector3(7f, 0f, 7f));
            SendRequest();

            Run();

            var newFormation = FindFormationOtherThan(oldFormation);
            Assert.That(_formationStash.Get(newFormation).Slots, Is.EqualTo(new[] { oldSlot }));
            Assert.That(_formationStash.Get(oldFormation).Slots, Is.Empty);
            Assert.That(_formationStash.Get(oldFormation).Units, Is.Empty);
            VectorAssert.AreEqual(new Vector3(7f, 0f, 7f), _positionStash.Get(oldSlot).Value);
            Assert.That(_world.GetStash<MoveSlotAssetPath>().Has(oldSlot), Is.False);
        }

        [Test]
        public void OnUpdate_NoSelectedUnits_CreatesNoFormationButHidesPreview()
        {
            CreatePreviewSlot(Vector3.zero);
            SendRequest();

            Run();

            Assert.That(_world.CountWith<MoveDestinationFormation>(), Is.Zero);
            Assert.That(_world.CountWith<HideMovePreviewSlotsRequest>(), Is.EqualTo(1));
        }

        [Test]
        public void OnUpdate_WithoutRequest_DoesNothing()
        {
            CreateSelectedUnit();
            CreatePreviewSlot(Vector3.zero);

            Run();

            Assert.That(_world.CountWith<MoveDestinationFormation>(), Is.Zero);
            Assert.That(_world.CountWith<HideMovePreviewSlotsRequest>(), Is.Zero);
        }

        private Entity CreateSelectedUnit()
        {
            var unit = _world.CreateEntity();
            _world.GetStash<UnitTag>().Set(unit);
            _world.GetStash<SelectedTag>().Set(unit);
            return unit;
        }

        private void CreatePreviewSlot(Vector3 position)
        {
            var slot = _world.CreateEntity();
            _world.GetStash<MovePreviewSlotTag>().Set(slot);
            _positionStash.Set(slot, new MoveSlotPosition { Value = position });
        }

        private void SendRequest()
        {
            _world.GetStash<CommitMoveRequest>().Set(_world.CreateEntity(),
                new CommitMoveRequest { OrderDirection = OrderDirection });
        }

        private Entity FindFormationOtherThan(Entity excluded)
        {
            foreach (var formation in _world.Filter.With<MoveDestinationFormation>().Build())
            {
                if (formation != excluded)
                    return formation;
            }
            throw new AssertionException("New formation was not created");
        }

        private void Run()
        {
            _world.Commit();
            _system.OnUpdate(DT);
            _world.Commit();
        }
    }
}
