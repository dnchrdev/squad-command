using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Features.GameplayECS.Common.View;
using Features.GameplayECS.UnitView.Systems.Ports;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Features.GameplayECS.UnitView.InterfaceAdapters.Gateways
{
    public sealed class AddressableUnitViewFactory : IUnitViewFactory
    {
        private readonly Dictionary<MonoEntity, AsyncOperationHandle<GameObject>> _handles = new();

        public async UniTask<MonoEntity> CreateAsync(string assetPath, Vector3 position, Quaternion rotation)
        {
            var handle = Addressables.InstantiateAsync(assetPath, position, rotation);
            GameObject go = await handle;
            
            var view = go.GetComponent<MonoEntity>();
            _handles[view] = handle;
            
            return view;
        }

        public void Release(MonoEntity view)
        {
            if (_handles.TryGetValue(view, out var handle))
            {
                Addressables.ReleaseInstance(handle);
                _handles.Remove(view);
            }
        }
    }
}