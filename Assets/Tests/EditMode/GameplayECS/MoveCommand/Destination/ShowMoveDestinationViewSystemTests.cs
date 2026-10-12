using Features.GameplayECS.Common.Components;
using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Destination.Components;
using Features.GameplayECS.MoveCommand.Destination.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.GameplayECS.MoveCommand.Fakes;
using Tests.EditMode.TestUtils;

namespace Tests.EditMode.GameplayECS.MoveCommand.Destination
{
    public class ShowMoveDestinationViewSystemTests
    {
        private const float DT = 0.016f;

        private World _world;
        private ShowMoveDestinationViewSystem _system;
        private SpyMoveSlotView _view;
        private Entity _slot;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _system = new ShowMoveDestinationViewSystem().Init(_world);
            _view = new SpyMoveSlotView();
            _slot = _world.CreateEntity();
            _world.GetStash<MoveDestinationSlotTag>().Set(_slot);
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
        public void OnUpdate_SlotStillHasAssetPath_IsNotShown()
        {
            _world.GetStash<AssetPath>().Set(_slot, new AssetPath { Value = "MoveSlotDestination" });
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_view.ShowCalls, Is.Zero);
        }
    }
}
