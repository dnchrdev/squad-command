using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.Systems
{
    /// <summary>
    /// Единственная система, отвечающая за физическое положение уже существующих
    /// превью-слотов (MovePreviewState.ActiveSlots). Каждый кадр, если сетка
    /// (MovePreviewState.LastGrid) была пересчитана заново (GridDirty),
    /// перекладывает позицию каждого активного слота на LastGrid[i] — то есть
    /// строго под последний известный formationForward/formationRight.
    ///
    /// Вынесена отдельно от MovePreviewShowSystem, чтобы:
    /// - MovePreviewShowSystem отвечала только за "сколько слотов нужно"
    ///   (геометрия количества/сетки), не трогая физическое размещение;
    /// - позиционирование слотов, созданных синхронно и асинхронно
    ///   (через MovePreviewCreateSystem), шло через один и тот же код
    ///   и было гарантированно согласовано с последним состоянием формации,
    ///   а не с состоянием на момент создания слота.
    /// </summary>
    public class MovePreviewSlotRepositionSystem : ISystem
    {
        public World World { get; set; }

        private Filter _stateFilter;

        private Stash<MovePreviewState> _stateStash;
        private Stash<MovePreviewSlotPosition> _slotPositionStash;
        private Stash<MovePreviewViewComponent> _viewStash;

        public void OnAwake()
        {
            _stateFilter = World.Filter.With<MovePreviewState>().Build();

            _stateStash = World.GetStash<MovePreviewState>();
            _slotPositionStash = World.GetStash<MovePreviewSlotPosition>();
            _viewStash = World.GetStash<MovePreviewViewComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            
            foreach (var stateEntity in _stateFilter)
            {
                ref var state = ref _stateStash.Get(stateEntity);
                
                if (!state.GridDirty)
                    continue;

                var activeSlots = state.ActiveSlots;
                var lastGrid = state.LastGrid;
                Debug.Log($"Reposition: dirty={state.GridDirty}, slots={activeSlots.Count}");

                // Переставляем позицию только тех слотов, для которых уже есть
                // соответствующий индекс в свежей сетке. Слоты за пределами
                // lastGrid.Count (если такие остались) — это те, что вот-вот
                // будут убраны MovePreviewShowSystem на следующем запросе,
                // либо ещё не созданы (их индексы обработает MovePreviewCreateSystem).
                int count = Mathf.Min(activeSlots.Count, lastGrid.Count);

                for (int i = 0; i < count; i++)
                {
                    RepositionSlot(activeSlots[i], lastGrid[i]);
                }

                state.GridDirty = false;
            }
        }

        private void RepositionSlot(Entity slotEntity, Vector3 position)
        {
            Debug.Log(
                $"Reposition slot: hasPos={_slotPositionStash.Has(slotEntity)}, hasView={_viewStash.Has(slotEntity)}, newPos={position}");
            
            if (_slotPositionStash.Has(slotEntity))
            {
                ref var slotPosition = ref _slotPositionStash.Get(slotEntity);
                slotPosition.Value = position;
            }

            if (_viewStash.Has(slotEntity))
            {
                _viewStash.Get(slotEntity).Value.SetPosition(position);
            }
        }

        public void Dispose()
        {
        }
    }
}