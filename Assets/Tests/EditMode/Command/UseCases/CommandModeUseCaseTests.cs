using Features.Command.Entities;
using Features.Command.UseCases;
using NUnit.Framework;
using Tests.EditMode.Command.UseCases.Fakes;

namespace Tests.EditMode.Command.UseCases
{
    public class CommandModeUseCaseTests
    {
        private const float SINGLE_SELECT_THRESHOLD = 25f;

        private SpyCommandModeOutput _mode;
        private SpySelectionFrameOutput _frame;
        private SpyMoveCommands _moveCommands;
        private SelectCommandUseCase _select;
        private MoveCommandUseCase _move;
        private CommandModeUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _mode = new SpyCommandModeOutput();
            _frame = new SpySelectionFrameOutput();
            _moveCommands = new SpyMoveCommands();
            _select = new SelectCommandUseCase(new StubScreenUnitQuery(), new SpySelectionCommands(), _frame, _mode,
                new CommandSettings(SINGLE_SELECT_THRESHOLD));
            _move = new MoveCommandUseCase(new StubUnitQuery(), new SpyMoveDragOutput(), new StubGroundQuery(),
                _moveCommands, _mode);
            _useCase = new CommandModeUseCase(_select, _move);
        }

        [Test]
        public void Current_BeforeAnyMode_IsNull()
        {
            Assert.That(_useCase.Current, Is.Null);
        }

        [Test]
        public void SetSelectMode_EntersSelect()
        {
            _useCase.SetSelectMode();

            Assert.That(_useCase.Current, Is.SameAs(_select));
            Assert.That(_mode.Shown, Is.EqualTo(new[] { CommandType.SELECT }));
        }

        [Test]
        public void SetMoveMode_FromSelect_ExitsSelectAndEntersMove()
        {
            _useCase.SetSelectMode();

            _useCase.SetMoveMode();

            Assert.That(_useCase.Current, Is.SameAs(_move));
            Assert.That(_mode.Shown, Is.EqualTo(new[] { CommandType.SELECT, CommandType.MOVE }));
            Assert.That(_frame.Calls, Is.EqualTo(new[] { "HideFrame" }));
        }

        [Test]
        public void SetSelectMode_FromMove_ClearsMovePreview()
        {
            _useCase.SetMoveMode();

            _useCase.SetSelectMode();

            Assert.That(_moveCommands.Calls, Is.EqualTo(new[] { "ClearPreview" }));
        }

        [Test]
        public void SetSelectMode_Twice_EntersOnce()
        {
            _useCase.SetSelectMode();

            _useCase.SetSelectMode();

            Assert.That(_mode.Shown, Is.EqualTo(new[] { CommandType.SELECT }));
            Assert.That(_frame.Calls, Is.Empty);
        }
    }
}
