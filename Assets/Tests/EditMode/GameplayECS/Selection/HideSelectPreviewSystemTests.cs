using Features.GameplayECS.Selection.Components;
using Features.GameplayECS.Selection.Outputs;
using Features.GameplayECS.Selection.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;

namespace Tests.EditMode.GameplayECS.Selection
{
    public class HideSelectPreviewSystemTests
    {
        private const float DT = 0.016f;

        private SelectionTestWorld _test;
        private World _world;
        private HideSelectPreviewSystem _system;

        [SetUp]
        public void SetUp()
        {
            _test = new SelectionTestWorld();
            _world = _test.World;
            _system = new HideSelectPreviewSystem().Init(_world);
        }

        [TearDown]
        public void TearDown() => _test.Dispose();

        [Test]
        public void OnUpdate_ShownUnitWithoutSelection_HidesViewAndClearsShownTag()
        {
            var unit = _test.CreateUnitWithView();
            _test.ShowVisual(unit);
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_test.IsVisualShown(unit), Is.False);
            Assert.That(_world.GetStash<SelectedViewShowedTag>().Has(unit), Is.False);
        }

        [Test]
        public void OnUpdate_ShownUnitInUnselectPreview_HidesView()
        {
            var unit = _test.CreateUnitWithView();
            _world.GetStash<SelectedTag>().Set(unit);
            _world.GetStash<UnselectPreviewTag>().Set(unit);
            _test.ShowVisual(unit);
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_test.IsVisualShown(unit), Is.False);
        }

        [Test]
        public void OnUpdate_ShownSelectedUnit_StaysVisible()
        {
            var unit = _test.CreateUnitWithView();
            _world.GetStash<SelectedTag>().Set(unit);
            _test.ShowVisual(unit);
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_test.IsVisualShown(unit), Is.True);
        }
    }
}
