using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Preview.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.GameplayECS.MoveCommand.Fakes;
using Tests.EditMode.TestUtils;

namespace Tests.EditMode.GameplayECS.MoveCommand.Preview
{
    public class ShowMovePreviewSystemTests
    {
        private const float DT = 0.016f;

        private World _world;
        private ShowMovePreviewSystem _system;
        private SpyMoveSlotView _view;
        private Entity _slot;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _system = new ShowMovePreviewSystem().Init(_world);
            _view = new SpyMoveSlotView();
            _slot = _world.CreateEntity();
            _world.GetStash<MovePreviewSlotTag>().Set(_slot);
            _world.GetStash<MoveSlotViewComponent>().Set(_slot, new MoveSlotViewComponent { Value = _view });
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_SlotWithView_ShowsAndMarksShown()
        {
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_view.ShowCalls, Is.EqualTo(1));
            Assert.That(_world.GetStash<MoveSlotViewShowedTag>().Has(_slot), Is.True);
        }

        [Test]
        public void OnUpdate_AlreadyShown_DoesNotShowAgain()
        {
            _world.GetStash<MoveSlotViewShowedTag>().Set(_slot);
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_view.ShowCalls, Is.Zero);
        }

        [Test]
        public void OnUpdate_SlotStillHasAssetPath_IsNotShown()
        {
            _world.GetStash<MoveSlotAssetPath>().Set(_slot, new MoveSlotAssetPath { Value = "MoveSlotPreview" });
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_view.ShowCalls, Is.Zero);
        }
    }
}
