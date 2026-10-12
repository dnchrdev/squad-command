using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Features.GameplayECS.Common.View;
using Features.GameplayECS.UnitView.Systems.Ports;
using UnityEngine;

namespace Tests.EditMode.GameplayECS.UnitView.Fakes
{
    public sealed class StubUnitViewFactory : IUnitViewFactory
    {
        public MonoEntity NextView;
        public bool Pending;

        public readonly List<string> RequestedPaths = new();
        public readonly List<MonoEntity> Released = new();
        public Vector3 LastPosition;
        public Quaternion LastRotation;

        private UniTaskCompletionSource<MonoEntity> _pending;

        public UniTask<MonoEntity> CreateAsync(string assetPath, Vector3 position, Quaternion rotation)
        {
            RequestedPaths.Add(assetPath);
            LastPosition = position;
            LastRotation = rotation;

            if (Pending == false)
                return UniTask.FromResult(NextView);

            _pending = new UniTaskCompletionSource<MonoEntity>();
            return _pending.Task;
        }

        public void CompletePending() => _pending.TrySetResult(NextView);

        public void Release(MonoEntity view) => Released.Add(view);
    }
}
