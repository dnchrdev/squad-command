using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Feature.GameplayECS.Common;
using Feature.GameplayECS.Spawning.UnitFactory;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.View.Systems
{
    public class CreateViewSystem : ISystem
    {
        private readonly IUnitViewFactory _unitViewFactory;
        private readonly Queue<Entity> _queue = new();
        private bool _isProcessing;

        public World World { get; set; }

        private Filter _needView;
        private Stash<AssetPath> _assetPathStash;
        private Stash<Position> _positionStash;
        private Stash<Rotation> _rotationStash;
        private Stash<View> _viewStash;

        public CreateViewSystem(IUnitViewFactory unitViewFactory)
        {
            _unitViewFactory = unitViewFactory;
        }

        public void OnAwake()
        {
            _needView = World.Filter
                .With<AssetPath>()
                .With<Position>()
                .With<Rotation>()
                .Without<View>()
                .Build();

            _assetPathStash = World.GetStash<AssetPath>();
            _positionStash = World.GetStash<Position>();
            _rotationStash = World.GetStash<Rotation>();
            _viewStash = World.GetStash<View>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _needView)
            {
                _viewStash.Add(entity);
                _queue.Enqueue(entity);
            }

            if (!_isProcessing && _queue.Count > 0)
            {
                ProcessQueueAsync().Forget();
            }
        }

        private async UniTask ProcessQueueAsync()
        {
            _isProcessing = true;

            while (_queue.Count > 0)
            {
                var entity = _queue.Dequeue();

                if (entity.IsDisposed())
                {
                    continue;
                }
                
                string assetPath = _assetPathStash.Get(entity).Value;
                Vector3 position = _positionStash.Get(entity).Value;
                Quaternion rotation = _rotationStash.Get(entity).Value;

                var view = await _unitViewFactory.CreateAsync(assetPath, position, rotation);

                if (entity.IsDisposed())
                {
                    _unitViewFactory.Release(view);
                    continue;
                }

                view.Bind(entity, World);
                _viewStash.Set(entity, new View { Value = view });
            }

            _isProcessing = false;
        }

        public void Dispose()
        {
        }
    }
}