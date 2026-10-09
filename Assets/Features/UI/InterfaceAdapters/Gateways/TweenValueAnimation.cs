using DG.Tweening;
using Features.Shared.UI;

namespace Features.UI.InterfaceAdapters.Gateways
{
    public sealed class TweenValueAnimation : IValueAnimation
    {
        private readonly Tween _tween;

        public TweenValueAnimation(Tween tween)
        {
            _tween = tween;
        }

        public void Kill() => _tween.Kill();
    }
}