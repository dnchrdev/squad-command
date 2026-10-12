using Features.GameplayECS.Common.Components;
using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Destination.Components;
using Features.GameplayECS.MoveCommand.Destination.Systems;
using Features.GameplayECS.MoveCommand.View;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.GameplayECS.MoveCommand.Fakes;
using Tests.EditMode.TestUtils;

namespace Tests.EditMode.GameplayECS.MoveCommand.Destination
{
    public class CreateMoveDestinationViewSystemTests
    {
        private const float DT = 0.016f;
        private const string DESTINATION_SLOT_ASSET_PATH = "MoveSlotDestination";

        private World _world;
        private FakeObjectPool<IMoveSlotView> _pool;
        private StubMoveSlotViewFactory _factory;
        private CreateMoveDestinationViewSystem _system;
        private Entity _slot;

        [SetUp]
        public void SetUp()
        {
            _world = World.Create();
            _pool = new FakeObjectPool<IMoveSlotView>();
            _factory = new StubMoveSlotViewFactory();
            _system = new CreateMoveDestinationViewSystem(_pool, _factory).Init(_world);

            _slot = _world.CreateEntity();
            _world.GetStash<MoveDestinationSlotTag>().Set(_slot);
            _world.GetStash<AssetPath>().Set(_slot, new AssetPath { Value = DESTINATION_SLOT_ASSET_PATH });
            _world.Commit();
        }

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_PoolHasFreeView_UsesItWithoutLoading()
        {
            var pooled = new SpyMoveSlotView();
            _pool.Prewarm(pooled);

            _system.OnUpdate(DT);

            Assert.That(_world.GetStash<MoveSlotViewComponent>().Get(_slot).Value, Is.SameAs(pooled));
            Assert.That(_factory.RequestedPaths, Is.Empty);
        }

        [Test]
        public void OnUpdate_PoolEmpty_LoadsHiddenViewFromFactory()
        {
            _system.OnUpdate(DT);

            Assert.That(_factory.RequestedPaths, Is.EqualTo(new[] { DESTINATION_SLOT_ASSET_PATH }));
            Assert.That(_world.GetStash<MoveSlotViewComponent>().Get(_slot).Value, Is.SameAs(_factory.NextView));
            Assert.That(_factory.NextView.IsVisible, Is.False);
        }

        [Test]
        public void OnUpdate_SlotRemovedWhileLoading_ReturnsViewToPool()
        {
            _factory.Pending = true;
            _system.OnUpdate(DT);

            _world.RemoveEntity(_slot);
            _world.Commit();
            _factory.CompletePending();

            Assert.That(_pool.Returned, Is.EqualTo(new[] { _factory.NextView }));
        }
    }
}
