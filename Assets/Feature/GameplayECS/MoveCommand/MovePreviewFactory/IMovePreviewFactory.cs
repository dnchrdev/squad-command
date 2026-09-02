using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.MovePreviewFactory
{
    public interface IMovePreviewFactory
    {
        UniTask<MovePreviewView> CreateAsync(string assetPath, Vector3 position, Quaternion rotation);
        void Release(MovePreviewView view);
    }
}