using System;
using System.Collections.Generic;
using Features.Command.Entities;
using Features.Command.InterfaceAdapters.Ports;
using Features.Command.UseCases.Ports;
using Features.Shared.UI;

namespace Features.Command.InterfaceAdapters.Presenters
{
    public sealed class CommandButtonsPresenter : ICommandModeOutput
    {
        private const float ELEVATION_ANIMATION_SPEED = 1000f;

        private readonly ICommandView _view;
        private readonly IValueAnimator _animator;
        private readonly Dictionary<CommandType, IValueAnimation> _animations = new();

        public CommandButtonsPresenter(ICommandView view, IValueAnimator animator)
        {
            _view = view;
            _animator = animator;
        }

        public void ShowActiveCommand(CommandType active)
        {
            foreach (CommandType type in Enum.GetValues(typeof(CommandType)))
                Animate(type, type == active ? _view.GetHighHeight() : _view.GetLowHeight());
        }

        private void Animate(CommandType type, float target)
        {
            if (_animations.TryGetValue(type, out var existing)) existing.Kill();
            var start = _view.GetButtonHeight(type);
            _animations[type] = _animator.AnimateBySpeed(start, target, ELEVATION_ANIMATION_SPEED,
                value => _view.SetButtonHeightOffset(type, value));
        }
    }
}