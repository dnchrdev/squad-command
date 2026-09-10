using Cysharp.Threading.Tasks;
using Feature.GameplayECS.MoveCommand.Adapter;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.MovePreviewFactory
{
    public interface IMoveSlotViewFactory
    {
        UniTask<IMoveSlotView> CreateAsync(string assetPath, Vector3 position, Quaternion rotation);
        void Release(IMoveSlotView view);
    }
}