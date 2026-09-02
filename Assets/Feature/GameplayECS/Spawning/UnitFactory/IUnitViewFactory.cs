using Cysharp.Threading.Tasks;
using Feature.GameplayECS.View;
using UnityEngine;

namespace Feature.GameplayECS.Spawning.UnitFactory
{
    public interface IUnitViewFactory
    {
        UniTask<MonoEntity> CreateAsync(string assetPath, Vector3 position, Quaternion rotation);
        void Release(MonoEntity view);
    }
}