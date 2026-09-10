using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Feature.Core.Infrastructure.Interfaces;
using Feature.GameplayECS.Common;
using Feature.GameplayECS.MoveCommand.Adapter;
using Feature.GameplayECS.MoveCommand.MovePreviewFactory;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.Systems.Preview.Systems
{
    public class CreateMovePreviewViewSystem : ISystem
    {
        private readonly Queue<Entity> _pendingSlots = new();
        private bool _isProcessing;

        private readonly IObjectPool<IMoveSlotView> _pool;
        private readonly IMoveSlotViewFactory _moveSlotViewFactory;

        public World World { get; set; }

        private Filter _needViewSlotFilter;

        private Stash<AssetPath> _assetPathStash;
        private Stash<MoveSlotViewComponent> _moveSlotViewStash;
        private Stash<Common.SpawningTag> _spawningTagStash;

        public CreateMovePreviewViewSystem(IObjectPool<IMoveSlotView> pool, IMoveSlotViewFactory moveSlotViewFactory)
        {
            _pool = pool;
            _moveSlotViewFactory = moveSlotViewFactory;
        }

        public void OnAwake()
        {
            _needViewSlotFilter = World.Filter
                .With<MovePreviewSlot>()
                .With<AssetPath>()
                .Without<MoveSlotViewComponent>()
                .Without<Common.SpawningTag>()
                .Build();

            _assetPathStash = World.GetStash<AssetPath>();
            _moveSlotViewStash = World.GetStash<MoveSlotViewComponent>();
            _spawningTagStash = World.GetStash<SpawningTag>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var neededViewSlot in _needViewSlotFilter)
            {
                if (_pool.FreeCount > 0)
                {
                    if (_pool.TryGet(out var pooledView))
                    {
                        _assetPathStash.Remove(neededViewSlot);
                        _moveSlotViewStash.Set(neededViewSlot, new MoveSlotViewComponent { Value = pooledView });
                        continue;
                    }
                }
                
                _spawningTagStash.Set(neededViewSlot);
                _pendingSlots.Enqueue(neededViewSlot);
            }

            if (!_isProcessing && _pendingSlots.Count > 0)
                ProcessPendingViewsAsync().Forget();
        }

        private async UniTask ProcessPendingViewsAsync()
        {
            _isProcessing = true;
            while (_pendingSlots.Count > 0)
            {
                var slot = _pendingSlots.Dequeue();
                
                if (World.IsDisposed(slot))
                {
                    continue;
                }
                
                string assetPath = _assetPathStash.Get(slot).Value;

                Vector3 position = Vector3.zero;
                Quaternion rotation = Quaternion.identity;

                IMoveSlotView view = await _moveSlotViewFactory.CreateAsync(assetPath, position, rotation);
                view.Hide();
                
                if (World.IsDisposed(slot))
                {
                    _pool.Return(view);
                    continue;
                }
                
                _spawningTagStash.Remove(slot);
                _assetPathStash.Remove(slot);
                
                _moveSlotViewStash.Set(slot, new MoveSlotViewComponent { Value = view });
            }

            _isProcessing = false;
        }


        public void Dispose()
        {
        }
    }
}