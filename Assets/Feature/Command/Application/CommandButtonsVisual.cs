using System;
using System.Collections.Generic;
using DG.Tweening;
using Feature.Command.Application.Interfaces;
using Feature.Command.Domain.Data;
using Feature.UI.Application;
using Zenject;

namespace Feature.Command.Application
{
    public sealed class CommandButtonsVisual
    {
        private const float ELEVATION_ANIMATION_SPEED = 1000f;
        [Inject] private readonly ICommandView _commandView;
        [Inject] private UIAnimator _uiAnimator;
        private readonly Dictionary<CommandType, Tween> _tweens = new();

        public void SetActiveButton(CommandType active)
        {
            foreach (CommandType type in Enum.GetValues(typeof(CommandType)))
                Animate(type, type == active ? _commandView.GetHighHeight() : _commandView.GetLowHeight());
        }

        private void Animate(CommandType type, float target)
        {
            if (_tweens.TryGetValue(type, out var existing)) existing.Kill();
            var start = _commandView.GetButtonHeight(type);
            _tweens[type] = _uiAnimator.AnimateFromToBySpeed(start, target,
                value => _commandView.SetButtonHeightOffset(type, value), ELEVATION_ANIMATION_SPEED, false);
        }
    }
}