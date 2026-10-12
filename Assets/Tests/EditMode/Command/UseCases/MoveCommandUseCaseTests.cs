using Features.Command.Entities;
using Features.Command.UseCases;
using Features.Shared.Units;
using NUnit.Framework;
using Scellecs.Morpeh;
using Tests.EditMode.Command.UseCases.Fakes;
using Tests.EditMode.TestUtils;
using UnityEngine;

namespace Tests.EditMode.Command.UseCases
{
    public class MoveCommandUseCaseTests
    {
        private static readonly Vector2 DownScreen = new Vector2(200f, 100f);
        private static readonly Vector2 MoveScreen = new Vector2(260f, 100f);
        private static readonly Vector3 DragStartGround = new Vector3(1f, 0f, 10f);
        private static readonly Vector3 DragEndGround = new Vector3(5f, 0f, 10f);

        private StubUnitQuery _unitQuery;
        private SpyMoveDragOutput _drag;
        private StubGroundQuery _ground;
        private SpyMoveCommands _move;
        private SpyCommandModeOutput _mode;
        private MoveCommandUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _unitQuery = new StubUnitQuery();
            _unitQuery.Selected.Add(new UnitSnapshot(default(Entity), new Vector3(0f, 0f, 0f)));
            _unitQuery.Selected.Add(new UnitSnapshot(default(Entity), new Vector3(2f, 0f, 0f)));
            _drag = new SpyMoveDragOutput();
            _ground = new StubGroundQuery { Point = DragStartGround };
            _move = new SpyMoveCommands();
            _mode = new SpyCommandModeOutput();
            _useCase = new MoveCommandUseCase(_unitQuery, _drag, _ground, _move, _mode);
        }

        [Test]
        public void OnEnter_ShowsMoveCommand()
        {
            _useCase.OnEnter();

            Assert.That(_mode.Shown, Is.EqualTo(new[] { CommandType.MOVE }));
        }

        [Test]
        public void PointerDown_OnGround_ShowsDragAndStartsPreview()
        {
            _useCase.OnPointerDown(DownScreen);

            Assert.That(_drag.Calls, Is.EqualTo(new[] { "ShowDrag" }));
            VectorAssert.AreEqual(DownScreen, _drag.LastStart);
            Assert.That(_move.Calls, Is.EqualTo(new[] { "StartPreview", "UpdatePreview" }));
        }

        [Test]
        public void PointerDown_OnGround_PreviewFacesAwayFromUnits()
        {
            _useCase.OnPointerDown(DownScreen);

            VectorAssert.AreEqual(new Vector2(1f, 10f), _move.LastCenter);
            VectorAssert.AreEqual(new Vector2(0f, 1f), _move.LastForward);
            VectorAssert.AreEqual(new Vector2(1f, 0f), _move.LastRight);
            VectorAssert.AreEqual(Vector2.zero, _move.LastSize);
        }

        [Test]
        public void PointerDown_OffGround_DoesNothing()
        {
            _ground.Hit = false;

            _useCase.OnPointerDown(DownScreen);

            Assert.That(_drag.Calls, Is.Empty);
            Assert.That(_move.Calls, Is.Empty);
        }

        [Test]
        public void PointerMoveAndUp_AfterPointerDownOffGround_DoNothing()
        {
            _ground.Hit = false;
            _useCase.OnPointerDown(DownScreen);

            _useCase.OnPointerMove(MoveScreen);
            _useCase.OnPointerUp(MoveScreen);

            Assert.That(_drag.Calls, Is.Empty);
            Assert.That(_move.Calls, Is.Empty);
        }

        [Test]
        public void PointerMove_OnGround_RotatesAndResizesPreview()
        {
            _useCase.OnPointerDown(DownScreen);
            _ground.Point = DragEndGround;

            _useCase.OnPointerMove(MoveScreen);

            VectorAssert.AreEqual(new Vector2(1f, 10f), _move.LastCenter);
            VectorAssert.AreEqual(new Vector2(-1f, 0f), _move.LastForward);
            VectorAssert.AreEqual(new Vector2(0f, 1f), _move.LastRight);
            VectorAssert.AreEqual(new Vector2(0f, 4f), _move.LastSize);
        }

        [Test]
        public void PointerMove_OnGround_StretchesDragFromPointerDown()
        {
            _useCase.OnPointerDown(DownScreen);

            _useCase.OnPointerMove(MoveScreen);

            Assert.That(_drag.Calls, Is.EqualTo(new[] { "ShowDrag", "UpdateDrag" }));
            VectorAssert.AreEqual(DownScreen, _drag.LastStart);
            VectorAssert.AreEqual(MoveScreen, _drag.LastEnd);
        }

        [Test]
        public void PointerMove_OffGround_KeepsLastValidGroundPoint()
        {
            _useCase.OnPointerDown(DownScreen);
            _ground.Point = DragEndGround;
            _useCase.OnPointerMove(MoveScreen);
            _ground.Hit = false;
            _ground.Point = new Vector3(100f, 0f, 100f);

            _useCase.OnPointerMove(MoveScreen);

            VectorAssert.AreEqual(new Vector2(0f, 4f), _move.LastSize);
            VectorAssert.AreEqual(new Vector2(-1f, 0f), _move.LastForward);
        }

        [Test]
        public void PointerUp_HidesDragAndCommitsTowardFormationCenter()
        {
            _useCase.OnPointerDown(DownScreen);

            _useCase.OnPointerUp(DownScreen);

            Assert.That(_drag.Calls, Is.EqualTo(new[] { "ShowDrag", "HideDrag" }));
            Assert.That(_move.Calls, Is.EqualTo(new[] { "StartPreview", "UpdatePreview", "CommitMove" }));
            VectorAssert.AreEqual(new Vector2(0f, 1f), _move.LastOrderDirection);
        }

        [Test]
        public void PointerUp_Twice_CommitsOnce()
        {
            _useCase.OnPointerDown(DownScreen);

            _useCase.OnPointerUp(DownScreen);
            _useCase.OnPointerUp(DownScreen);

            Assert.That(_move.Calls.FindAll(call => call == "CommitMove"), Has.Count.EqualTo(1));
        }

        [Test]
        public void Canceled_HidesDragClearsPreviewAndSkipsCommit()
        {
            _useCase.OnPointerDown(DownScreen);

            _useCase.Canceled();
            _useCase.OnPointerUp(DownScreen);

            Assert.That(_drag.Calls, Is.EqualTo(new[] { "ShowDrag", "HideDrag" }));
            Assert.That(_move.Calls, Is.EqualTo(new[] { "StartPreview", "UpdatePreview", "ClearPreview" }));
        }

        [Test]
        public void OnExit_HidesDragAndClearsPreview()
        {
            _useCase.OnPointerDown(DownScreen);

            _useCase.OnExit();

            Assert.That(_drag.Calls, Is.EqualTo(new[] { "ShowDrag", "HideDrag" }));
            Assert.That(_move.Calls, Is.EqualTo(new[] { "StartPreview", "UpdatePreview", "ClearPreview" }));
        }
    }
}
