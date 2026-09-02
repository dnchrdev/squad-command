using Feature.Core.Infrastructure.Interfaces;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.MoveCommand.Systems
{
    /// <summary>
    /// Обрабатывает HideMovePreviewRequest: прячет все текущие показанные слоты превью
    /// (MovePreviewShowed), возвращает их вьюшки в пул и уничтожает слот-сущности.
    /// </summary>
    public class MovePreviewHideSystem : ISystem
    {
        public World World { get; set; }

        private Filter _hideRequestFilter;
        private Filter _stateFilter;

        private Stash<HideMovePreviewRequest> _hideRequestStash;
        private Stash<MovePreviewViewComponent> _viewStash;
        private Stash<MovePreviewShowed> _showedStash;
        private Stash<MovePreviewTag> _tagStash;
        private Stash<MovePreviewSlotPosition> _slotPositionStash;
        private Stash<MovePreviewState> _stateStash;

        private readonly IObjectPool<MovePreviewView> _pool;

        public MovePreviewHideSystem(IObjectPool<MovePreviewView> pool)
        {
            _pool = pool;
        }

        public void OnAwake()
        {
            _hideRequestFilter = World.Filter
                .With<HideMovePreviewRequest>()
                .Build();

            _stateFilter = World.Filter
                .With<MovePreviewState>()
                .Build();

            _hideRequestStash = World.GetStash<HideMovePreviewRequest>();
            _viewStash = World.GetStash<MovePreviewViewComponent>();
            _showedStash = World.GetStash<MovePreviewShowed>();
            _tagStash = World.GetStash<MovePreviewTag>();
            _slotPositionStash = World.GetStash<MovePreviewSlotPosition>();
            _stateStash = World.GetStash<MovePreviewState>();
        }

        public void OnUpdate(float deltaTime)
        {
            bool hasHideRequest = false;

            foreach (var requestEntity in _hideRequestFilter)
            {
                hasHideRequest = true;

                _hideRequestStash.Remove(requestEntity);
                World.RemoveEntity(requestEntity);
            }

            if (!hasHideRequest)
                return;

            foreach (var stateEntity in _stateFilter)
            {
                ref var state = ref _stateStash.Get(stateEntity);
                var activeSlots = state.ActiveSlots;

                for (int i = 0; i < activeSlots.Count; i++)
                {
                    HideAndRecycleSlot(activeSlots[i]);
                }

                activeSlots.Clear();
            }
        }

        private void HideAndRecycleSlot(Entity slotEntity)
        {
            if (_viewStash.Has(slotEntity))
            {
                var view = _viewStash.Get(slotEntity).Value;
                view.Hide();
                _pool.Return(view);
                _viewStash.Remove(slotEntity);
            }

            if (_showedStash.Has(slotEntity))
                _showedStash.Remove(slotEntity);

            if (_slotPositionStash.Has(slotEntity))
                _slotPositionStash.Remove(slotEntity);

            if (_tagStash.Has(slotEntity))
                _tagStash.Remove(slotEntity);

            World.RemoveEntity(slotEntity);
        }

        public void Dispose()
        {
        }
    }
}