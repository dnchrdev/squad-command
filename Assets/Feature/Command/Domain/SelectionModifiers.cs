using Feature.Command.Domain.Data;

namespace Feature.Command.Domain
{
    public sealed class SelectionModifiers
    {
        private bool _shift;
        private bool _ctrl;

        public SelectionMode Mode => _shift ? SelectionMode.Add
            : _ctrl ? SelectionMode.Remove
            : SelectionMode.Replace;

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