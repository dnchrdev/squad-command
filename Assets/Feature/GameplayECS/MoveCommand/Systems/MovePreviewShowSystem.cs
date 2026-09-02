using System.Collections.Generic;
using Feature.Core.Infrastructure.Interfaces;
using Feature.GameplayECS.Common;
using Feature.GameplayECS.MoveCommand.MovePreviewFactory;
using Feature.GameplayECS.SelectCommand;
using Feature.GameplayECS.View;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.Systems
{
  public class MovePreviewShowSystem : ISystem
    {
        public World World { get; set; }

        private const float SlotSpacing = 1.2f;
        private const float MinBlendDiagonal = 1.5f;
        private const float MaxBlendDiagonal = 6f;

        private Filter _requestFilter;
        private Filter _stateFilter;
        private Filter _selectedFilter;

        private Stash<MovePreviewRequest> _requestStash;
        private Stash<MovePreviewViewComponent> _viewStash;
        private Stash<MovePreviewShowed> _showedStash;
        private Stash<MovePreviewState> _stateStash;
        private Stash<CreatePreviewSlotRequest> _createRequestStash;

        private readonly IObjectPool<MovePreviewView> _pool;

        private readonly List<Vector3> _slotsBuffer = new List<Vector3>(64);

        private Entity _stateEntity;

        public MovePreviewShowSystem(IObjectPool<MovePreviewView> pool)
        {
            _pool = pool;
        }

        public void OnAwake()
        {
            _requestFilter = World.Filter.With<MovePreviewRequest>().Build();
            _stateFilter = World.Filter.With<MovePreviewState>().Build();
            _selectedFilter = World.Filter.With<UnitTag>().With<Selected>().Build();

            _requestStash = World.GetStash<MovePreviewRequest>();
            _viewStash = World.GetStash<MovePreviewViewComponent>();
            _showedStash = World.GetStash<MovePreviewShowed>();
            _stateStash = World.GetStash<MovePreviewState>();
            _createRequestStash = World.GetStash<CreatePreviewSlotRequest>();

            EnsureStateEntity();
        }

        public void OnUpdate(float deltaTime)
        {
            bool hasRequest = false;
            MovePreviewRequest lastRequest = default;

            foreach (var requestEntity in _requestFilter)
            {
                hasRequest = true;
                lastRequest = _requestStash.Get(requestEntity);

                _requestStash.Remove(requestEntity);
                World.RemoveEntity(requestEntity);
            }

            if (!hasRequest)
                return;

            int selectedUnitsCount = CountSelectedUnits();
            Debug.Log($"OnUpdate: hasRequest={hasRequest}, selectedCount={selectedUnitsCount}");
            if (selectedUnitsCount <= 0)
                return;
            
            ApplyRequest(lastRequest, selectedUnitsCount);
        }

        private int CountSelectedUnits()
        {
            int count = 0;
            foreach (var _ in _selectedFilter)
                count++;
            return count;
        }

        private void ApplyRequest(in MovePreviewRequest request, int unitsCount)
        {
            Debug.Log($"ApplyRequest: forward={request.FormationForward}");
            
            GenerateAdaptiveGrid(
                unitsCount,
                request.Center,
                request.Size,
                request.FormationForward,
                request.FormationRight,
                _slotsBuffer);

            ref var state = ref _stateStash.Get(_stateEntity);

            // Сохраняем свежую сетку — это единственный источник истины
            // для позиции слота, и для уже существующих (repositioning
            // system), и для создающихся асинхронно (create system).
            state.LastGrid.Clear();
            state.LastGrid.AddRange(_slotsBuffer);
            state.GridDirty = true;

            var activeSlots = state.ActiveSlots;

            if (activeSlots.Count > _slotsBuffer.Count)
            {
                for (int i = activeSlots.Count - 1; i >= _slotsBuffer.Count; i--)
                {
                    HideSlot(activeSlots[i]);
                    activeSlots.RemoveAt(i);
                }
            }
            else if (activeSlots.Count < _slotsBuffer.Count)
            {
                // Недостающие позиции — не создаём вьюшку тут, а публикуем заявку
                // с ИНДЕКСОМ в LastGrid (не финальной позицией). MovePreviewCreateSystem
                // подхватит её и сама резолвит актуальную позицию по индексу в момент,
                // когда вьюшка реально готова (это может случиться позже текущего кадра,
                // если требуется асинхронная фабрика, — и за это время LastGrid мог
                // ещё раз обновиться из-за вращения формации).
                for (int i = activeSlots.Count; i < _slotsBuffer.Count; i++)
                {
                    var requestEntity = World.CreateEntity();
                    _createRequestStash.Set(requestEntity, new CreatePreviewSlotRequest
                    {
                        SlotIndex = i,
                        Position = _slotsBuffer[i]
                    });
                }
            }
        }

        private void HideSlot(Entity slotEntity)
        {
            if (_viewStash.Has(slotEntity))
            {
                var view = _viewStash.Get(slotEntity).Value;
                view.Hide();
                _pool.Return(view);
            }

            if (_showedStash.Has(slotEntity))
                _showedStash.Remove(slotEntity);

            World.RemoveEntity(slotEntity);
        }

        private void EnsureStateEntity()
        {
            foreach (var entity in _stateFilter)
            {
                _stateEntity = entity;
                return;
            }

            _stateEntity = World.CreateEntity();
            _stateStash.Set(_stateEntity, new MovePreviewState
            {
                ActiveSlots = new List<Entity>(64),
                LastGrid = new List<Vector3>(64),
                GridDirty = false
            });
        }

        // ----------------------------------------------------------------
        // Расчёт адаптивной сетки слотов под прямоугольник выделения.
        // ----------------------------------------------------------------

        private static void GenerateAdaptiveGrid(
            int unitsCount,
            Vector2 center,
            Vector2 size,
            Vector2 formationForward,
            Vector2 formationRight,
            List<Vector3> result)
        {
            result.Clear();

            if (unitsCount <= 0)
                return;

            Vector2 right = formationRight.sqrMagnitude > 0.0001f ? formationRight.normalized : Vector2.right;
            Vector2 forward = formationForward.sqrMagnitude > 0.0001f ? formationForward.normalized : Vector2.up;

            ResolveGridDimensions(unitsCount, size, out int columns, out int rows);

            const float spacing = SlotSpacing;

            int placed = 0;
            for (int row = rows - 1; row >= 0 && placed < unitsCount; row--)
            {
                float forwardOffset = (row - (rows - 1) * 0.5f) * spacing;

                for (int col = 0; col < columns && placed < unitsCount; col++)
                {
                    float rightOffset = (col - (columns - 1) * 0.5f) * spacing;

                    Vector2 flatOffset = right * rightOffset + forward * forwardOffset;
                    Vector2 flatPosition = center + flatOffset;

                    result.Add(new Vector3(flatPosition.x, 0f, flatPosition.y));
                    placed++;
                }
            }
        }

        private static void ResolveGridDimensions(int unitsCount, Vector2 size, out int columns, out int rows)
        {
            float width = Mathf.Max(size.x, 0.01f);
            float height = Mathf.Max(size.y, 0.01f);
            float rawAspect = width / height;

            float diagonal = Mathf.Sqrt(size.x * size.x + size.y * size.y);
            float blend = Mathf.InverseLerp(MinBlendDiagonal, MaxBlendDiagonal, diagonal);

            float blendedAspect = Mathf.Exp(Mathf.Lerp(0f, Mathf.Log(rawAspect), blend));

            int baseSide = Mathf.CeilToInt(Mathf.Sqrt(unitsCount));

            columns = baseSide;
            rows = Mathf.CeilToInt(unitsCount / (float)columns);

            float bestScore = float.PositiveInfinity;
            int bestColumns = columns;
            int bestRows = rows;

            int maxDimension = unitsCount;
            for (int c = 1; c <= maxDimension; c++)
            {
                int r = Mathf.CeilToInt(unitsCount / (float)c);
                if (c * r < unitsCount)
                    continue;

                float candidateAspect = c / (float)r;
                float score = Mathf.Abs(Mathf.Log(candidateAspect) - Mathf.Log(blendedAspect));

                if (score < bestScore)
                {
                    bestScore = score;
                    bestColumns = c;
                    bestRows = r;
                }
            }

            columns = bestColumns;
            rows = bestRows;
        }

        public void Dispose()
        {
        }
    }
}