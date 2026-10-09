
namespace Features.Command.Entities
{
    public sealed class SelectionModifiers
    {
        private bool _shift;
        private bool _ctrl;

        public SelectionMode Mode => _shift ? SelectionMode.ADD
            : _ctrl ? SelectionMode.REMOVE
            : SelectionMode.REPLACE;

        public void SetShift(bool value)
        {
            _shift = value;
            if (value) _ctrl = false;
        }

        public void SetCtrl(bool value)
        {
            _ctrl = value;
            if (value) _shift = false;
        }
    }
}