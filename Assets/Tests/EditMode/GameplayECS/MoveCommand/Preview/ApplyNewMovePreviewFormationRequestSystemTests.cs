using System.Collections.Generic;
using Features.GameplayECS.Common.Components;
using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Preview.Components;
using Features.GameplayECS.MoveCommand.Preview.Requests;
using Features.GameplayECS.MoveCommand.Preview.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;

namespace Tests.EditMode.GameplayECS.MoveCommand.Preview
{
    public class ApplyNewMovePreviewFormationRequestSystemTests
    {
        private const float DT = 0.016f;
        private const string PREVIEW_SLOT_ASSET_PATH = "MoveSlotPreview";

        private World _world;
        private ApplyNewMovePreviewFormationRequestSystem _system;
        private Stash<MovePreviewFormation> _formationStash;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _system = new ApplyNewMovePreviewFormationRequestSystem().Init(_world);
            _formationStash = _world.GetStash<MovePreviewFormation>();
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_CreatesPreviewSlotPerSelectedUnit()
        {
            CreateSelectedUnit();
            CreateSelectedUnit();
            _world.CreateEntity();
            SendRequest();

            Run();

            var slots = _formationStash.Get(_world.SingleWith<MovePreviewFormation>()).Slots;
            Assert.That(slots, Has.Count.EqualTo(2));
            foreach (var slot in slots)
            {
                Assert.That(_world.GetStash<MovePreviewSlotTag>().Has(slot), Is.True);
                Assert.That(_world.GetStash<AssetPath>().Get(slot).Value, Is.EqualTo(PREVIEW_SLOT_ASSET_PATH));
            }
        }

        [Test]
        public void OnUpdate_ConsumesRequest()
        {
            CreateSelectedUnit();
            SendRequest();

            Run();

            Assert.That(_world.CountWith<NewMovePreviewFormationRequest>(), Is.Zero);
        }

        [Test]
        public void OnUpdate_NoSelectedUnits_CreatesNoFormation()
        {
            SendRequest();

            Run();

            Assert.That(_world.CountWith<MovePreviewFormation>(), Is.Zero);
            Assert.That(_world.CountWith<NewMovePreviewFormationRequest>(), Is.Zero);
        }

        [Test]
        public void OnUpdate_ExistingFormation_IsReused()
        {
            var existing = _world.CreateEntity();
            _formationStash.Set(existing, new MovePreviewFormation { Slots = new List<Entity>() });
            CreateSelectedUnit();
            SendRequest();

            Run();

            Assert.That(_world.CountWith<MovePreviewFormation>(), Is.EqualTo(1));
            Assert.That(_formationStash.Get(existing).Slots, Has.Count.EqualTo(1));
        }

        [Test]
        public void OnUpdate_WithoutRequest_DoesNothing()
        {
            CreateSelectedUnit();

            Run();

            Assert.That(_world.CountWith<MovePreviewFormation>(), Is.Zero);
            Assert.That(_world.CountWith<MovePreviewSlotTag>(), Is.Zero);
        }

        private void CreateSelectedUnit()
        {
            var unit = _world.CreateEntity();
            _world.GetStash<UnitTag>().Set(unit);
            _world.GetStash<SelectedTag>().Set(unit);
        }

        private void SendRequest() => _world.GetStash<NewMovePreviewFormationRequest>().Set(_world.CreateEntity());

        private void Run()
        {
            _world.Commit();
            _system.OnUpdate(DT);
            _world.Commit();
        }
    }
}
