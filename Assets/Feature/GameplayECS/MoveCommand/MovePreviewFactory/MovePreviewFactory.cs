using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Feature.GameplayECS.View;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Feature.GameplayECS.MoveCommand.MovePreviewFactory
{
    public class MovePreviewFactory: IMovePreviewFactory
    {
        private readonly Dictionary<MovePreviewView, AsyncOperationHandle<GameObject>> _handles = new();

        public async UniTask<MovePreviewView> CreateAsync(string assetPath, Vector3 position, Quaternion rotation)
        {
            var handle = Addressables.InstantiateAsync(assetPath, position, rotation);
            GameObject go = await handle;
            
            var view = go.GetComponent<MovePreviewView>();
            _handles[view] = handle;
            
            return view;
        }
        
        public void Release(MovePreviewView view)
        {
            if (_handles.TryGetValue(view, out var handle))
            {
                Addressables.ReleaseInstance(handle);
                _handles.Remove(view);
            }
        }
    }
}