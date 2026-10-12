using Features.GameplayECS.Common.Components;
using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Preview.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.GameplayECS.MoveCommand.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.MoveCommand.Preview
{
    public class SyncMovePreviewSlotViewSystemTests
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
            var system = new SyncMovePreviewSlotViewSystem().Init(_world);
            var view = new SpyMoveSlotView();
            var slot = _world.CreateEntity();
            _world.GetStash<MovePreviewSlotTag>().Set(slot);
            _world.GetStash<Position>().Set(slot, new Position { Value = new Vector3(3f, 0.01f, 4f) });
            _world.GetStash<MoveSlotViewComponent>().Set(slot, new MoveSlotViewComponent { Value = view });
            _world.Commit();

            system.OnUpdate(DT);

            VectorAssert.AreEqual(new Vector3(3f, 0.01f, 4f), view.Position);
        }
    }
}
