using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Features.GameplayECS.MoveCommand.Ports;
using Features.GameplayECS.MoveCommand.View;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.MoveCommand.Fakes
{
    public sealed class StubMoveSlotViewFactory : IMoveSlotViewFactory
    {
        public readonly SpyMoveSlotView NextView = new();
        public bool Pending;

        public readonly List<string> RequestedPaths = new();
        public readonly List<IMoveSlotView> Released = new();

        private UniTaskCompletionSource<IMoveSlotView> _pending;

        public UniTask<IMoveSlotView> CreateAsync(string assetPath, Vector3 position, Quaternion rotation)
        {
            RequestedPaths.Add(assetPath);

            if (Pending == false)
                return UniTask.FromResult<IMoveSlotView>(NextView);

            _pending = new UniTaskCompletionSource<IMoveSlotView>();
            return _pending.Task;
        }

        public void CompletePending() => _pending.TrySetResult(NextView);

        public void Release(IMoveSlotView view) => Released.Add(view);
    }
}
