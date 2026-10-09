using System;
using DG.Tweening;
using Features.Shared.UI;
using UnityEngine;

namespace Features.UI.InterfaceAdapters.Gateways
{
    public sealed class ValueAnimatorGateway : IValueAnimator
    {
        public IValueAnimation AnimateBySpeed(float from, float to, float speed, Action<float> onUpdate)
        {
            float distance = Mathf.Abs(to - from);
            float duration = speed > 0f ? distance / speed : 0f;

            var tween = DOTween.To(() => from, x => from = x, to, duration)
                .OnUpdate(() => onUpdate(from))
                .SetEase(Ease.Linear);

            return new TweenValueAnimation(tween);
        }
    }
}