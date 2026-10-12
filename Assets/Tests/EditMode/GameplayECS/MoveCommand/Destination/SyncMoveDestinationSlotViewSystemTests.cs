using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Destination.Components;
using Features.GameplayECS.MoveCommand.Destination.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.GameplayECS.MoveCommand.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.MoveCommand.Destination
{
    public class SyncMoveDestinationSlotViewSystemTests
    {
        private const float DT = 0.016f;

        private World _world;

        [SetUp]
        public void SetUp() => _world = World.Create();

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void OnUpdate_MovesViewToSlotPosition()
        {
            var system = new SyncMoveDestinationSlotViewSystem().Init(_world);
            var view = new SpyMoveSlotView();
            var slot = _world.CreateEntity();
            _world.GetStash<MoveDestinationSlotTag>().Set(slot);
            _world.GetStash<MoveSlotPosition>().Set(slot, new MoveSlotPosition { Value = new Vector3(3f, 0f, 4f) });
            _world.GetStash<MoveSlotViewComponent>().Set(slot, new MoveSlotViewComponent { Value = view });
            _world.Commit();

            system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(3f, 0f, 4f), view.Position);
        }
    }
}
