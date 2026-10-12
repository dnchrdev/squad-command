using System.Collections.Generic;
using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Preview.Components;
using Features.GameplayECS.MoveCommand.Preview.Requests;
using Features.GameplayECS.MoveCommand.Preview.Systems;
using Features.GameplayECS.MoveCommand.View;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.GameplayECS.MoveCommand.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.MoveCommand.Preview
{
    public class ApplyHideMovePreviewSlotsRequestSystemTests
    {
        private const float DT = 0.016f;

        private World _world;
        private FakeObjectPool<IMoveSlotView> _pool;
        private ApplyHideMovePreviewSlotsRequestSystem _system;
        private SpyMoveSlotView _shownView;
        private Entity _formation;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _pool = new FakeObjectPool<IMoveSlotView>();
            _system = new ApplyHideMovePreviewSlotsRequestSystem(_pool).Init(_world);

            _shownView = new SpyMoveSlotView();
            var shownSlot = CreateSlot();
            _world.GetStash<MoveSlotPosition>().Set(shownSlot, new MoveSlotPosition { Value = Vector3.zero });
            _world.GetStash<MoveSlotViewComponent>().Set(shownSlot, new MoveSlotViewComponent { Value = _shownView });
            _world.GetStash<MoveSlotViewShowedTag>().Set(shownSlot);

            var loadingSlot = CreateSlot();
            _world.GetStash<MoveSlotAssetPath>().Set(loadingSlot, new MoveSlotAssetPath { Value = "MoveSlotPreview" });
            _world.GetStash<MoveSlotSpawningTag>().Set(loadingSlot);

            _formation = _world.CreateEntity();
            _world.GetStash<MovePreviewFormation>().Set(_formation,
                new MovePreviewFormation { Slots = new List<Entity> { shownSlot, loadingSlot } });
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_HidesViewsAndReturnsThemToPool()
        {
            SendRequest();

            Run();

            Assert.That(_shownView.IsVisible, Is.False);
            Assert.That(_pool.Returned, Is.EqualTo(new[] { _shownView }));
        }

        [Test]
        public void OnUpdate_RemovesAllSlots()
        {
            SendRequest();

            Run();

            Assert.That(_world.GetStash<MovePreviewFormation>().Get(_formation).Slots, Is.Empty);
            Assert.That(_world.CountWith<MovePreviewSlotTag>(), Is.Zero);
        }

        [Test]
        public void OnUpdate_ConsumesRequest()
        {
            SendRequest();

            Run();

            Assert.That(_world.CountWith<HideMovePreviewSlotsRequest>(), Is.Zero);
        }

        [Test]
        public void OnUpdate_WithoutRequest_KeepsSlots()
        {
            Run();

            Assert.That(_world.CountWith<MovePreviewSlotTag>(), Is.EqualTo(2));
            Assert.That(_shownView.IsVisible, Is.True);
        }

        private Entity CreateSlot()
        {
            var slot = _world.CreateEntity();
            _world.GetStash<MovePreviewSlotTag>().Set(slot);
            return slot;
        }

        private void SendRequest() => _world.GetStash<HideMovePreviewSlotsRequest>().Set(_world.CreateEntity());

        private void Run()
        {
            _world.Commit();
            _system.OnUpdate(DT);
            _world.Commit();
        }
    }
}
