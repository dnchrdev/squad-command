using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Feature.GameplayECS.MoveCommand.Adapter;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Feature.GameplayECS.MoveCommand.MovePreviewFactory
{
    public class MoveSlotViewFactory : IMoveSlotViewFactory
    {
        private readonly Dictionary<IMoveSlotView, AsyncOperationHandle<GameObject>> _handles = new();

        public async UniTask<IMoveSlotView> CreateAsync(string assetPath, Vector3 position, Quaternion rotation)
        {
            var handle = Addressables.InstantiateAsync(assetPath, position, rotation);
            GameObject go = await handle;

            var view = go.GetComponent<IMoveSlotView>();
            _handles[view] = handle;

            return view;
        }

        public void Release(IMoveSlotView view)
        {
            if (_handles.TryGetValue(view, out var handle))
            {
                Addressables.ReleaseInstance(handle);
                _handles.Remove(view);
            }
        }
    }
}