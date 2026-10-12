using Features.Command.Entities;
using NUnit.Framework;

namespace Tests.EditMode.Command.Entities
{
    public class SelectionModifiersTests
    {
        [TestCase(false, false, SelectionMode.REPLACE)]
        [TestCase(true, false, SelectionMode.ADD)]
        [TestCase(false, true, SelectionMode.REMOVE)]
        public void Mode_DependsOnHeldKeys(bool shift, bool ctrl, SelectionMode expected)
        {
            var modifiers = new SelectionModifiers();

            modifiers.SetShift(shift);
            modifiers.SetCtrl(ctrl);

            Assert.That(modifiers.Mode, Is.EqualTo(expected));
        }

        [Test]
        public void SetCtrl_WhileShiftHeld_SwitchesToRemove()
        {
            var modifiers = new SelectionModifiers();
            modifiers.SetShift(true);

            modifiers.SetCtrl(true);

            Assert.That(modifiers.Mode, Is.EqualTo(SelectionMode.REMOVE));
        }

        [Test]
        public void SetShift_WhileCtrlHeld_SwitchesToAdd()
        {
            var modifiers = new SelectionModifiers();
            modifiers.SetCtrl(true);

            modifiers.SetShift(true);

            Assert.That(modifiers.Mode, Is.EqualTo(SelectionMode.ADD));
        }

        [Test]
        public void SetShift_Released_ReturnsToReplace()
        {
            var modifiers = new SelectionModifiers();
            modifiers.SetShift(true);

            modifiers.SetShift(false);

            Assert.That(modifiers.Mode, Is.EqualTo(SelectionMode.REPLACE));
        }
    }
}
