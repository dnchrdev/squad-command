using R3;
using UnityEngine;

namespace Feature.Input.Adapter.Interfaces
{
    public interface IReadOnlyCameraInput
    {
        ReadOnlyReactiveProperty<Vector2> PointerDelta { get; }
        Observable<float> Zoom { get; }
        Observable<Unit> DragStarted { get; }
        ReadOnlyReactiveProperty<bool> IsDragging { get; }
        Observable<Unit> RotationStarted { get; }
        ReadOnlyReactiveProperty<bool> IsRotating { get; }
        
    }
}