using R3;
using UnityEngine;

namespace Feature.Input.Adapter.Interfaces
{
    public interface IReadOnlyCameraInput
    {
        ReadOnlyReactiveProperty<Vector2> PointerDelta { get; }
        Observable<float> Zoom { get; }
        ReadOnlyReactiveProperty<bool> IsDragging { get; }
        ReadOnlyReactiveProperty<bool> IsRotating { get; }

    }
}