using Cysharp.Threading.Tasks;
using Features.GameplayECS.Common.View;
using UnityEngine;

namespace Features.GameplayECS.UnitView.Systems.Ports
{
    public interface IUnitViewFactory
    {
        UniTask<MonoEntity> CreateAsync(string assetPath, Vector3 position, Quaternion rotation);
        void Release(MonoEntity view);
    }
}