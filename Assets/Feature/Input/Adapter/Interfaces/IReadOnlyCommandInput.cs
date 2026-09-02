using R3;
using UnityEngine;

namespace Feature.Input.Adapter.Interfaces
{
    public interface IReadOnlyCommandInput
    {
        ReadOnlyReactiveProperty<Vector2> PointerPosition { get; }
        Observable<Unit> ClickDown { get; }
        Observable<Unit> ClickUp { get; }
        
        Observable<Unit> ShiftDown { get; }
        Observable<Unit> ShiftUp { get; }
        
        Observable<Unit> CtrlDown { get; }
        Observable<Unit> CtrlUp { get; }
        
        Observable<Unit> Select { get; }
        Observable<Unit> Move { get; }
        Observable<Unit> Attack { get; }
    }
}
