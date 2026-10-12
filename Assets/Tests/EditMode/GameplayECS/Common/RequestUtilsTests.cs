using Features.GameplayECS.Common;
using Features.GameplayECS.Common.Components;
using Features.GameplayECS.MoveCommand.Preview.Requests;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;

namespace Tests.EditMode.GameplayECS.Common
{
    public class RequestUtilsTests
    {
        private World _world;

        [SetUp]
        public void SetUp() => _world = World.Create();

        [TearDown]
        public void TearDown() => _world.Dispose();

        [Test]
        public void ConsumeAllRequests_RemovesRequestEntitiesOnly()
        {
            var stash = _world.GetStash<HideMovePreviewSlotsRequest>();
            stash.Set(_world.CreateEntity());
            stash.Set(_world.CreateEntity());
            _world.GetStash<UnitTag>().Set(_world.CreateEntity());
            var filter = _world.Filter.With<HideMovePreviewSlotsRequest>().Build();
            _world.Commit();

            RequestUtils.ConsumeAllRequests(_world, filter, stash);
            _world.Commit();

            Assert.That(_world.CountWith<HideMovePreviewSlotsRequest>(), Is.Zero);
            Assert.That(_world.CountWith<UnitTag>(), Is.EqualTo(1));
        }
    }
}
