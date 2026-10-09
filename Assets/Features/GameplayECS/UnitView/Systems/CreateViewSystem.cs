using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Features.GameplayECS.Common.Components;
using Features.GameplayECS.UnitView.Components;
using Features.GameplayECS.UnitView.Systems.Ports;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.UnitView.Systems
{
    public class CreateViewSystem : ISystem
    {
        private readonly IUnitViewFactory _unitViewFactory;
        private readonly Queue<Entity> _pendingViews = new();
        private bool _isProcessing;
        public World World { get; set; }
        private Filter _needViewFilter;
        private Stash<AssetPath> _assetPathStash;
        private Stash<Position> _positionStash;
        private Stash<Rotation> _rotationStash;
        private Stash<UnitViewComponent> _viewStash;
        private Stash<SpawningTag> _spawningTagStash;

        public CreateViewSystem(IUnitViewFactory unitViewFactory)
        {
            _unitViewFactory = unitViewFactory;
        }

        public void OnAwake()
        {
            _needViewFilter = World.Filter
                .With<AssetPath>().With<Position>().With<Rotation>().Without<UnitViewComponent>().Without<SpawningTag>().Build();
            _assetPathStash = World.GetStash<AssetPath>();
            _positionStash = World.GetStash<Position>();
            _rotationStash = World.GetStash<Rotation>();
            _viewStash = World.GetStash<UnitViewComponent>();
            _spawningTagStash = World.GetStash<SpawningTag>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _needViewFilter)
            {
                _spawningTagStash.Set(entity);
                _pendingViews.Enqueue(entity);
            }

            if (!_isProcessing && _pendingViews.Count > 0)
                ProcessPendingViewsAsync().Forget();
        }

        private async UniTask ProcessPendingViewsAsync()
        {
            _isProcessing = true;
            while (_pendingViews.Count > 0)
            {
                var entity = _pendingViews.Dequeue();
                if (World.IsDisposed(entity)) continue;

                string assetPath = _assetPathStash.Get(entity).Value;
                Vector3 position = _positionStash.Get(entity).Value;
                Quaternion rotation = _rotationStash.Get(entity).Value;
                var view = await _unitViewFactory.CreateAsync(assetPath, position, rotation);

                if (World.IsDisposed(entity))
                {
                    _unitViewFactory.Release(view);
                    continue;
                }

                _spawningTagStash.Remove(entity);
                view.Bind(entity, World);
                _viewStash.Set(entity, new UnitViewComponent { Value = view });
            }
            _isProcessing = false;
        }

        public void Dispose()
        {
        }
    }
}