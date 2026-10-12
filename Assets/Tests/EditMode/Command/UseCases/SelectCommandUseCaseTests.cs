using Features.Command.Entities;
using Features.Command.UseCases;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.Command.UseCases.Fakes;
using UnityEngine;

namespace Tests.EditMode.Command.UseCases
{
    public class SelectCommandUseCaseTests
    {
        private const float SINGLE_SELECT_THRESHOLD = 25f;

        private static readonly Entity[] OneUnit = new Entity[1];

        private StubScreenUnitQuery _unitQuery;
        private SpySelectionCommands _commands;
        private SpySelectionFrameOutput _frame;
        private SpyCommandModeOutput _mode;
        private SelectCommandUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _unitQuery = new StubScreenUnitQuery { UnitsInRect = OneUnit };
            _commands = new SpySelectionCommands();
            _frame = new SpySelectionFrameOutput();
            _mode = new SpyCommandModeOutput();
            _useCase = new SelectCommandUseCase(
                _unitQuery,
                _commands,
                _frame,
                _mode,
                new CommandSettings(SINGLE_SELECT_THRESHOLD));
        }

        [Test]
        public void OnEnter_ShowsSelectCommand()
        {
            _useCase.OnEnter();

            Assert.That(_mode.Shown, Is.EqualTo(new[] { CommandType.SELECT }));
        }

        [Test]
        public void SetShiftHold_ClearsSelectionPreview()
        {
            _useCase.SetShiftHold(true);

            Assert.That(_commands.Calls, Is.EqualTo(new[] { "ClearSelectionPreview" }));
        }

        [Test]
        public void ClickOnEmptyGround_InReplaceMode_ClearsWithoutCommit()
        {
            _useCase.OnPointerDown(new Vector2(100f, 100f));
            _useCase.OnPointerUp(new Vector2(101f, 100f));

            Assert.That(_commands.Calls, Is.EqualTo(new[] { "ClearSelection" }));
        }

        [Test]
        public void ClickOnEmptyGround_WithShift_KeepsSelection()
        {
            _useCase.SetShiftHold(true);

            _useCase.OnPointerDown(new Vector2(100f, 100f));
            _useCase.OnPointerUp(new Vector2(101f, 100f));

            Assert.That(_commands.Calls, Is.EqualTo(new[] { "ClearSelectionPreview", "CommitSelection" }));
        }

        [Test]
        public void ClickOnUnit_InReplaceMode_ReplacesPreviewAndCommits()
        {
            _unitQuery.UnitsAtPoint = OneUnit;

            _useCase.OnPointerDown(new Vector2(100f, 100f));
            _useCase.OnPointerUp(new Vector2(101f, 100f));

            Assert.That(_commands.Calls, Is.EqualTo(new[] { "ReplaceSelectionPreview", "CommitSelection" }));
        }

        [Test]
        public void ClickOnUnit_WithCtrl_RemovesFromPreviewAndCommits()
        {
            _unitQuery.UnitsAtPoint = OneUnit;
            _useCase.SetCtrlHold(true);

            _useCase.OnPointerDown(new Vector2(100f, 100f));
            _useCase.OnPointerUp(new Vector2(101f, 100f));

            Assert.That(_commands.Calls,
                Is.EqualTo(new[] { "ClearSelectionPreview", "RemoveFromSelectionPreview", "CommitSelection" }));
        }

        [Test]
        public void Click_UsesThresholdAsPickTolerance()
        {
            _useCase.OnPointerDown(new Vector2(100f, 100f));
            _useCase.OnPointerUp(new Vector2(101f, 100f));

            Assert.That(_unitQuery.LastTolerance, Is.EqualTo(SINGLE_SELECT_THRESHOLD));
        }

        [Test]
        public void Drag_WithoutModifiers_ReplacesPreview()
        {
            _useCase.OnPointerDown(new Vector2(0f, 0f));

            _useCase.OnPointerMove(new Vector2(100f, 100f));

            Assert.That(_commands.Calls, Is.EqualTo(new[] { "ReplaceSelectionPreview" }));
        }

        [Test]
        public void DragWithShift_AddsUnitsToPreview()
        {
            _useCase.SetShiftHold(true);
            _useCase.OnPointerDown(new Vector2(0f, 0f));

            _useCase.OnPointerMove(new Vector2(100f, 100f));

            Assert.That(_commands.Calls, Is.EqualTo(new[] { "ClearSelectionPreview", "AddToSelectionPreview" }));
        }

        [Test]
        public void DragWithCtrl_RemovesUnitsFromPreview()
        {
            _useCase.SetCtrlHold(true);
            _useCase.OnPointerDown(new Vector2(0f, 0f));

            _useCase.OnPointerMove(new Vector2(100f, 100f));

            Assert.That(_commands.Calls, Is.EqualTo(new[] { "ClearSelectionPreview", "RemoveFromSelectionPreview" }));
        }

        [Test]
        public void PointerMove_ExactlyAtThreshold_DoesNotChangePreview()
        {
            _useCase.OnPointerDown(new Vector2(0f, 0f));

            _useCase.OnPointerMove(new Vector2(3f, 4f));

            Assert.That(_commands.Calls, Is.Empty);
        }

        [Test]
        public void PointerMove_JustPastThreshold_ReplacesPreview()
        {
            _useCase.OnPointerDown(new Vector2(0f, 0f));

            _useCase.OnPointerMove(new Vector2(3f, 4.1f));

            Assert.That(_commands.Calls, Is.EqualTo(new[] { "ReplaceSelectionPreview" }));
        }

        [Test]
        public void DragRelease_CommitsWithoutPickingSingleUnit()
        {
            _unitQuery.UnitsAtPoint = OneUnit;
            _useCase.OnPointerDown(new Vector2(0f, 0f));
            _useCase.OnPointerMove(new Vector2(100f, 100f));

            _useCase.OnPointerUp(new Vector2(100f, 100f));

            Assert.That(_commands.Calls, Is.EqualTo(new[] { "ReplaceSelectionPreview", "CommitSelection" }));
        }

        [Test]
        public void PointerMove_WithoutPointerDown_DoesNothing()
        {
            _useCase.OnPointerMove(new Vector2(100f, 100f));

            Assert.That(_commands.Calls, Is.Empty);
            Assert.That(_frame.Calls, Is.Empty);
        }

        [Test]
        public void PointerUp_WithoutPointerDown_DoesNothing()
        {
            _useCase.OnPointerUp(new Vector2(100f, 100f));

            Assert.That(_commands.Calls, Is.Empty);
            Assert.That(_frame.Calls, Is.Empty);
        }

        [Test]
        public void PointerDown_ShowsZeroSizeFrameAtPointer()
        {
            _useCase.OnPointerDown(new Vector2(100f, 50f));

            Assert.That(_frame.Calls, Is.EqualTo(new[] { "ShowFrame" }));
            Assert.That(_frame.LastRect, Is.EqualTo(new Rect(100f, 50f, 0f, 0f)));
        }

        [Test]
        public void PointerMove_StretchesFrameFromDragStartToPointer()
        {
            _useCase.OnPointerDown(new Vector2(100f, 50f));

            _useCase.OnPointerMove(new Vector2(40f, 80f));

            Assert.That(_frame.LastRect, Is.EqualTo(new Rect(40f, 50f, 60f, 30f)));
        }

        [Test]
        public void PointerUp_HidesFrame()
        {
            _useCase.OnPointerDown(new Vector2(100f, 50f));

            _useCase.OnPointerUp(new Vector2(100f, 50f));

            Assert.That(_frame.Calls, Is.EqualTo(new[] { "ShowFrame", "HideFrame" }));
        }

        [Test]
        public void Canceled_DuringDrag_HidesFrameAndClearsSelection()
        {
            _useCase.OnPointerDown(new Vector2(0f, 0f));
            _useCase.OnPointerMove(new Vector2(100f, 100f));

            _useCase.Canceled();

            Assert.That(_frame.Calls, Is.EqualTo(new[] { "ShowFrame", "UpdateFrame", "HideFrame" }));
            Assert.That(_commands.Calls, Is.EqualTo(new[] { "ReplaceSelectionPreview", "ClearSelection" }));
        }

        [Test]
        public void PointerUp_AfterCancel_DoesNotCommit()
        {
            _useCase.OnPointerDown(new Vector2(0f, 0f));
            _useCase.OnPointerMove(new Vector2(100f, 100f));
            _useCase.Canceled();

            _useCase.OnPointerUp(new Vector2(100f, 100f));

            Assert.That(_commands.Calls, Does.Not.Contain("CommitSelection"));
        }

        [Test]
        public void OnExit_DuringDrag_FinishesSelectionLikePointerUp()
        {
            _useCase.OnPointerDown(new Vector2(0f, 0f));
            _useCase.OnPointerMove(new Vector2(100f, 100f));

            _useCase.OnExit();

            Assert.That(_frame.Calls, Is.EqualTo(new[] { "ShowFrame", "UpdateFrame", "HideFrame" }));
            Assert.That(_commands.Calls, Is.EqualTo(new[] { "ReplaceSelectionPreview", "CommitSelection" }));
        }

        [Test]
        public void OnExit_AfterPressWithoutMove_ActsAsClick()
        {
            _useCase.OnPointerDown(new Vector2(100f, 50f));

            _useCase.OnExit();

            Assert.That(_commands.Calls, Is.EqualTo(new[] { "ClearSelection" }));
        }

        [Test]
        public void PointerUp_AfterExitDuringDrag_DoesNotCommitAgain()
        {
            _useCase.OnPointerDown(new Vector2(0f, 0f));
            _useCase.OnPointerMove(new Vector2(100f, 100f));
            _useCase.OnExit();

            _useCase.OnPointerUp(new Vector2(100f, 100f));

            Assert.That(_commands.Calls.FindAll(call => call == "CommitSelection"), Has.Count.EqualTo(1));
        }

        [Test]
        public void OnExit_WithoutDrag_OnlyHidesFrame()
        {
            _useCase.OnExit();

            Assert.That(_frame.Calls, Is.EqualTo(new[] { "HideFrame" }));
            Assert.That(_commands.Calls, Is.Empty);
        }
    }
}
