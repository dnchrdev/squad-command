using System;

namespace Features.Shared.UI
{
    public interface IValueAnimator
    {
        IValueAnimation AnimateBySpeed(float from, float to, float speed, Action<float> onUpdate);
    }
}