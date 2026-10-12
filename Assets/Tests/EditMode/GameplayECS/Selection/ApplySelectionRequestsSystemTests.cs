using System.Collections.Generic;
using Features.GameplayECS.Common.Components;
using Features.GameplayECS.Selection.Components;
using Features.GameplayECS.Selection.Requests;
using Features.GameplayECS.Selection.Systems;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.TestUtils;

namespace Tests.EditMode.GameplayECS.Selection
{
    public class ApplySelectionRequestsSystemTests
    {
        private const float DT = 0.016f;

        private SelectionTestWorld _test;
        private World _world;
        private ApplySelectionRequestsSystem _system;
        private Stash<SelectedTag> _selected;
        private Stash<SelectPreviewTag> _selectPreview;
        private Stash<UnselectPreviewTag> _unselectPreview;
        private int _sequence;

        [SetUp]
        public void SetUp()
        {
            _test = new SelectionTestWorld();
            _world = _test.World;
            _system = new ApplySelectionRequestsSystem().Init(_world);
            _selected = _world.GetStash<SelectedTag>();
            _selectPreview = _world.GetStash<SelectPreviewTag>();
            _unselectPreview = _world.GetStash<UnselectPreviewTag>();
            _sequence = 0;
        }

        [TearDown]
        public void TearDown() => _test.Dispose();

        [Test]
        public void PreviewAdd_MarksUnitsAsSelectPreview()
        {
            var unit = _test.CreateUnit();

            Run(SelectionRequestType.PreviewAdd, unit);

            Assert.That(_selectPreview.Has(unit), Is.True);
        }

        [Test]
        public void PreviewAdd_KeepsCurrentSelection()
        {
            var selected = _test.CreateUnit();
            _selected.Set(selected);
            var added = _test.CreateUnit();

            Run(SelectionRequestType.PreviewAdd, added);

            Assert.That(_selected.Has(selected), Is.True);
            Assert.That(_selectPreview.Has(added), Is.True);
        }

        [Test]
        public void PreviewAdd_ReplacesPreviousAddPreview()
        {
            var first = _test.CreateUnit();
            var second = _test.CreateUnit();
            Run(SelectionRequestType.PreviewAdd, first);

            Run(SelectionRequestType.PreviewAdd, second);

            Assert.That(_selectPreview.Has(first), Is.False);
            Assert.That(_selectPreview.Has(second), Is.True);
        }

        [Test]
        public void PreviewReplace_ClearsSelectionAndMarksNewPreview()
        {
            var selected = _test.CreateUnit();
            _selected.Set(selected);
            var replacement = _test.CreateUnit();

            Run(SelectionRequestType.PreviewReplace, replacement);

            Assert.That(_selected.Has(selected), Is.False);
            Assert.That(_selectPreview.Has(replacement), Is.True);
        }

        [Test]
        public void PreviewRemove_MarksUnitsAsUnselectPreview()
        {
            var unit = _test.CreateUnit();
            _selected.Set(unit);

            Run(SelectionRequestType.PreviewRemove, unit);

            Assert.That(_unselectPreview.Has(unit), Is.True);
            Assert.That(_selected.Has(unit), Is.True);
        }

        [Test]
        public void ClearSelection_RemovesSelectedAndSelectPreview()
        {
            var selected = _test.CreateUnit();
            _selected.Set(selected);
            var previewed = _test.CreateUnit();
            _selectPreview.Set(previewed);

            Run(SelectionRequestType.ClearSelection);

            Assert.That(_selected.Has(selected), Is.False);
            Assert.That(_selectPreview.Has(previewed), Is.False);
        }

        [Test]
        public void ClearPreview_RemovesBothPreviewsAndKeepsSelection()
        {
            var unit = _test.CreateUnit();
            _selected.Set(unit);
            _selectPreview.Set(unit);
            _unselectPreview.Set(unit);

            Run(SelectionRequestType.ClearPreview);

            Assert.That(_selectPreview.Has(unit), Is.False);
            Assert.That(_unselectPreview.Has(unit), Is.False);
            Assert.That(_selected.Has(unit), Is.True);
        }

        [Test]
        public void Commit_SelectPreview_SelectsUnitAndShowsView()
        {
            var unit = _test.CreateUnitWithView();
            _selectPreview.Set(unit);

            Run(SelectionRequestType.Commit);

            Assert.That(_selected.Has(unit), Is.True);
            Assert.That(_selectPreview.Has(unit), Is.False);
            Assert.That(_test.IsVisualShown(unit), Is.True);
            Assert.That(_world.GetStash<SelectedViewShowedTag>().Has(unit), Is.True);
        }

        [Test]
        public void Commit_UnselectPreview_DeselectsUnitAndHidesView()
        {
            var unit = _test.CreateUnitWithView();
            _selected.Set(unit);
            _test.ShowVisual(unit);
            _unselectPreview.Set(unit);

            Run(SelectionRequestType.Commit);

            Assert.That(_selected.Has(unit), Is.False);
            Assert.That(_unselectPreview.Has(unit), Is.False);
            Assert.That(_test.IsVisualShown(unit), Is.False);
            Assert.That(_world.GetStash<SelectedViewShowedTag>().Has(unit), Is.False);
        }

        [Test]
        public void Commit_UnitWithoutView_StaysInPreview()
        {
            var unit = _test.CreateUnit();
            _selectPreview.Set(unit);

            Run(SelectionRequestType.Commit);

            Assert.That(_selected.Has(unit), Is.False);
            Assert.That(_selectPreview.Has(unit), Is.True);
        }

        [Test]
        public void OnUpdate_AppliesRequestsInSequenceOrder()
        {
            var unit = _test.CreateUnitWithView();
            Send(SelectionRequestType.Commit, 1);
            Send(SelectionRequestType.PreviewAdd, 0, unit);
            _world.Commit();

            _system.OnUpdate(DT);

            Assert.That(_selected.Has(unit), Is.True);
        }

        [Test]
        public void OnUpdate_ConsumesAllRequests()
        {
            var unit = _test.CreateUnit();
            Send(SelectionRequestType.PreviewAdd, 0, unit);
            Send(SelectionRequestType.Commit, 1);
            _world.Commit();

            _system.OnUpdate(DT);
            _world.Commit();

            Assert.That(_world.CountWith<SelectionRequest>(), Is.Zero);
        }

        [Test]
        public void PreviewAdd_RemovedUnit_IsSkipped()
        {
            var alive = _test.CreateUnit();
            var removed = _test.CreateUnit();
            _world.Commit();
            _world.RemoveEntity(removed);

            Assert.DoesNotThrow(() => Run(SelectionRequestType.PreviewAdd, alive, removed));
            Assert.That(_selectPreview.Has(alive), Is.True);
        }

        private void Run(SelectionRequestType type, params Entity[] units)
        {
            Send(type, _sequence++, units);
            _world.Commit();
            _system.OnUpdate(DT);
            _world.Commit();
        }

        private void Send(SelectionRequestType type, int sequence, params Entity[] units)
        {
            _world.GetStash<SelectionRequest>().Set(_world.CreateEntity(), new SelectionRequest
            {
                Type = type,
                Units = new List<Entity>(units),
                Sequence = sequence
            });
        }
    }
}
