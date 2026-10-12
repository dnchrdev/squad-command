using Features.GameplayECS.Selection.Components;
using Features.GameplayECS.Selection.Outputs;
using Features.GameplayECS.Selection.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;

namespace Tests.EditMode.GameplayECS.Selection
{
    public class ShowSelectPreviewSystemTests
    {
        private const float DT = 0.016f;

        private SelectionTestWorld _test;
        private World _world;
        private ShowSelectPreviewSystem _system;

        [SetUp]
        public void SetUp()
        {
            _test = new SelectionTestWorld();
            _world = _test.World;
            _system = new ShowSelectPreviewSystem().Init(_world);
        }

        [TearDown]
        public void TearDown() => _test.Dispose();

        [Test]
        public void OnUpdate_SelectPreviewUnit_ShowsViewAndMarksShown()
        {
            var unit = _test.CreateUnitWithView();
            _world.GetStash<SelectPreviewTag>().Set(unit);
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_test.IsVisualShown(unit), Is.True);
            Assert.That(_world.GetStash<SelectedViewShowedTag>().Has(unit), Is.True);
        }

        [Test]
        public void OnUpdate_SelectedUnit_ShowsView()
        {
            var unit = _test.CreateUnitWithView();
            _world.GetStash<SelectedTag>().Set(unit);
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_test.IsVisualShown(unit), Is.True);
        }

        [Test]
        public void OnUpdate_SelectedUnitInUnselectPreview_StaysHidden()
        {
            var unit = _test.CreateUnitWithView();
            _world.GetStash<SelectedTag>().Set(unit);
            _world.GetStash<UnselectPreviewTag>().Set(unit);
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_test.IsVisualShown(unit), Is.False);
        }
    }
}
