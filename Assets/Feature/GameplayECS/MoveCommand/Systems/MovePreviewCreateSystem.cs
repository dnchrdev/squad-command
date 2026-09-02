using System.Collections.Generic;
using Feature.Core.Infrastructure.Interfaces;
using Feature.GameplayECS.MoveCommand.MovePreviewFactory;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.Systems
{
    /// <summary>
    /// Разбирает CreatePreviewSlotRequest: пытается синхронно взять вьюшку из пула;
    /// если пула не хватило — ставит позицию в очередь на создание через фабрику
    /// (Addressables, асинхронно) и создаёт slot-entity, когда вьюшка готова.
    /// Владеет всей асинхронной механикой создания — MovePreviewShowSystem об этом
    /// ничего не знает.
    /// </summary>
    public class MovePreviewCreateSystem : ISystem
    {
        public World World { get; set; }

        private const string MovePreviewAssetPath = "MoveSlotPreview";

        private Filter _createRequestFilter;
        private Filter _stateFilter;

        private Stash<CreatePreviewSlotRequest> _createRequestStash;
        private Stash<MovePreviewTag> _tagStash;
        private Stash<MovePreviewViewComponent> _viewStash;
        private Stash<MovePreviewShowed> _showedStash;
        private Stash<MovePreviewSlotPosition> _slotPositionStash;
        private Stash<MovePreviewState> _stateStash;

        private readonly IObjectPool<MovePreviewView> _pool;
        private readonly IMovePreviewFactory _factory;

        private readonly Queue<Vector3> _missingViewQueue = new Queue<Vector3>();
        private bool _isCreatingMissingView;

        private Entity _stateEntity;

        public MovePreviewCreateSystem(IObjectPool<MovePreviewView> pool, IMovePreviewFactory factory)
        {
            _pool = pool;
            _factory = factory;
        }

        public void OnAwake()
        {
            _createRequestFilter = World.Filter.With<CreatePreviewSlotRequest>().Build();
            _stateFilter = World.Filter.With<MovePreviewState>().Build();

            _createRequestStash = World.GetStash<CreatePreviewSlotRequest>();
            _tagStash = World.GetStash<MovePreviewTag>();
            _viewStash = World.GetStash<MovePreviewViewComponent>();
            _showedStash = World.GetStash<MovePreviewShowed>();
            _slotPositionStash = World.GetStash<MovePreviewSlotPosition>();
            _stateStash = World.GetStash<MovePreviewState>();

            EnsureStateEntity();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var requestEntity in _createRequestFilter)
            {
                ref var request = ref _createRequestStash.Get(requestEntity);
                HandleCreateRequest(request.Position);

                _createRequestStash.Remove(requestEntity);
                World.RemoveEntity(requestEntity);
            }
        }

        private void HandleCreateRequest(Vector3 position)
        {
            if (_pool.TryGet(out var view))
            {
                PlaceAndShow(view, position);
                var slotEntity = BuildSlotEntity(view, position);
                AddSlotToState(slotEntity);
                return;
            }

            EnqueueMissingView(position);
        }

        private void EnqueueMissingView(Vector3 position)
        {
            _missingViewQueue.Enqueue(position);
            ProcessMissingViewQueue();
        }

        private async void ProcessMissingViewQueue()
        {
            if (_isCreatingMissingView)
                return;

            _isCreatingMissingView = true;
            try
            {
                while (_missingViewQueue.Count > 0)
                {
                    var position = _missingViewQueue.Dequeue();

                    MovePreviewView view;
                    try
                    {
                        view = await _factory.CreateAsync(MovePreviewAssetPath, position, Quaternion.identity);
                    }
                    catch
                    {
                        continue;
                    }

                    if (view == null)
                        continue;

                    PlaceAndShow(view, position);
                    var slotEntity = BuildSlotEntity(view, position);
                    AddSlotToState(slotEntity);
                }
            }
            finally
            {
                _isCreatingMissingView = false;
            }
        }

        private void AddSlotToState(Entity slotEntity)
        {
            ref var state = ref _stateStash.Get(_stateEntity);
            state.ActiveSlots.Add(slotEntity);
        }

        private static void PlaceAndShow(MovePreviewView view, Vector3 position)
        {
            view.SetPosition(position);
            view.Show();
        }

        private Entity BuildSlotEntity(MovePreviewView view, Vector3 position)
        {
            var slotEntity = World.CreateEntity();

            _tagStash.Set(slotEntity, new MovePreviewTag());
            _viewStash.Set(slotEntity, new MovePreviewViewComponent { Value = view });
            _showedStash.Set(slotEntity, new MovePreviewShowed());
            _slotPositionStash.Set(slotEntity, new MovePreviewSlotPosition { Value = position });

            return slotEntity;
        }

        private void EnsureStateEntity()
        {
            foreach (var entity in _stateFilter)
            {
                _stateEntity = entity;
                return;
            }

            _stateEntity = World.CreateEntity();
            _stateStash.Set(_stateEntity, new MovePreviewState { ActiveSlots = new List<Entity>(64) });
        }

        public void Dispose()
        {
        }
    }
}