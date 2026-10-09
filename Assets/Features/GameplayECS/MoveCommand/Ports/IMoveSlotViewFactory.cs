using Cysharp.Threading.Tasks;
using Features.GameplayECS.MoveCommand.View;
using UnityEngine;

namespace Features.GameplayECS.MoveCommand.Ports
{
    public interface IMoveSlotViewFactory
    {
        UniTask<IMoveSlotView> CreateAsync(string assetPath, Vector3 position, Quaternion rotation);
        void Release(IMoveSlotView view);
    }
}