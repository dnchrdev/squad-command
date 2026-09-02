using System;
using System.Collections.Generic;
using Feature.GameplayECS.Common;
using Feature.GameplayECS.Navigation;
using Feature.GameplayECS.SelectCommand;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.Systems
{
/// <summary>
    /// Обрабатывает CommitMoveRequest. Назначение юнит -> слот делается жадно:
    /// 1) слоты сортируются по убыванию проекции на вектор направления формации
    ///    (FormationForward) относительно центроида отряда — самые дальние
    ///    ПО ХОДУ ДВИЖЕНИЯ слоты идут первыми;
    /// 2) идём по этому списку и для каждого слота забираем САМОГО БЛИЖАЙШЕГО
    ///    к нему юнита из ещё не занятых.
    ///
    /// Это даёт ровно нужный эффект: дальний по направлению движения угол
    /// формации достаётся тому, кто физически ближе всего к этому углу, а не
    /// тому, кто выгоднее по сумме всех расстояний отряда (в этом и была ошибка
    /// венгерского алгоритма — он решает другую задачу, глобальную сумму, а не
    /// "дальний слот -> ближайший юнит").
    /// </summary>
    public class CommitMoveSystem : ISystem
    {
        public World World { get; set; }

        private Filter _commitRequestFilter;
        private Filter _stateFilter;
        private Filter _selectedFilter;

        private Stash<CommitMoveRequest> _commitRequestStash;
        private Stash<MovePreviewState> _stateStash;
        private Stash<MovePreviewSlotPosition> _slotPositionStash;
        private Stash<Position> _positionStash;
        private Stash<DestinationRequest> _destinationRequestStash;
        private Stash<HideMovePreviewRequest> _hideRequestStash;

        private readonly List<Vector3> _slotsBuffer = new List<Vector3>(64);
        private readonly List<Entity> _unitsBuffer = new List<Entity>(64);

        // Переиспользуемые буферы для назначения, чтобы не аллоцировать каждый коммит.
        private readonly List<int> _slotOrderBuffer = new List<int>(64);     // индексы в _slotsBuffer, отсортированные по убыванию проекции на направление формации
        private readonly List<Vector3> _unitPositionsBuffer = new List<Vector3>(64);
        private readonly List<bool> _unitTakenBuffer = new List<bool>(64);

        public void OnAwake()
        {
            _commitRequestFilter = World.Filter.With<CommitMoveRequest>().Build();
            _stateFilter = World.Filter.With<MovePreviewState>().Build();
            _selectedFilter = World.Filter.With<UnitTag>().With<Selected>().Build();

            _commitRequestStash = World.GetStash<CommitMoveRequest>();
            _stateStash = World.GetStash<MovePreviewState>();
            _slotPositionStash = World.GetStash<MovePreviewSlotPosition>();
            _positionStash = World.GetStash<Position>();
            _destinationRequestStash = World.GetStash<DestinationRequest>();
            _hideRequestStash = World.GetStash<HideMovePreviewRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            bool hasCommit = false;
            Vector2 formationForward = Vector2.zero;

            foreach (var requestEntity in _commitRequestFilter)
            {
                hasCommit = true;
                formationForward = _commitRequestStash.Get(requestEntity).FormationForward;
                _commitRequestStash.Remove(requestEntity);
                World.RemoveEntity(requestEntity);
            }

            if (!hasCommit)
                return;

            CollectSelectedUnits(_unitsBuffer);
            if (_unitsBuffer.Count == 0)
                return;

            CollectPreviewSlots(_slotsBuffer);
            if (_slotsBuffer.Count == 0)
                return;

            AssignFarthestSlotsToNearestUnits(_unitsBuffer, _slotsBuffer, formationForward);
            IssueHidePreview();
        }

        private void CollectSelectedUnits(List<Entity> buffer)
        {
            buffer.Clear();
            foreach (var entity in _selectedFilter)
            {
                if (_positionStash.Has(entity))
                    buffer.Add(entity);
            }
        }

        private void CollectPreviewSlots(List<Vector3> buffer)
        {
            buffer.Clear();

            foreach (var stateEntity in _stateFilter)
            {
                ref var state = ref _stateStash.Get(stateEntity);
                var activeSlots = state.ActiveSlots;

                for (int i = 0; i < activeSlots.Count; i++)
                {
                    var slotEntity = activeSlots[i];
                    if (_slotPositionStash.Has(slotEntity))
                        buffer.Add(_slotPositionStash.Get(slotEntity).Value);
                }
            }
        }

        private void AssignFarthestSlotsToNearestUnits(List<Entity> units, List<Vector3> slots, Vector2 formationForward)
        {
            int unitsCount = units.Count;
            int slotsCount = slots.Count;

            _unitPositionsBuffer.Clear();
            _unitTakenBuffer.Clear();
            for (int i = 0; i < unitsCount; i++)
            {
                _unitPositionsBuffer.Add(_positionStash.Get(units[i]).Value);
                _unitTakenBuffer.Add(false);
            }

            Vector3 squadOrigin = ComputeCentroid(_unitPositionsBuffer);

            // Направление формации в плоскости XZ (Vector2.x -> world.x, Vector2.y -> world.z).
            // Нормализуем на случай, если вызывающий код передал ненормализованный вектор;
            // при нулевом/вырожденном векторе используем fallback, чтобы не ломать сортировку NaN'ами.
            Vector3 direction = new Vector3(formationForward.x, 0f, formationForward.y);
            if (direction.sqrMagnitude < 1e-6f)
                direction = Vector3.forward;
            else
                direction.Normalize();

            _slotOrderBuffer.Clear();
            for (int i = 0; i < slotsCount; i++)
                _slotOrderBuffer.Add(i);

            _slotOrderBuffer.Sort((a, b) =>
            {
                float projA = ProjectXZ(slots[a] - squadOrigin, direction);
                float projB = ProjectXZ(slots[b] - squadOrigin, direction);
                return projB.CompareTo(projA); // убывание: слоты дальше по направлению формации — первыми
            });

            int slotsToAssign = Mathf.Min(unitsCount, slotsCount);

            for (int order = 0; order < slotsToAssign; order++)
            {
                int slotIndex = _slotOrderBuffer[order];
                Vector3 slotPosition = slots[slotIndex];

                int nearestUnitIndex = FindNearestFreeUnit(slotPosition, _unitPositionsBuffer, _unitTakenBuffer);
                if (nearestUnitIndex < 0)
                    break;

                _unitTakenBuffer[nearestUnitIndex] = true;

                var unit = units[nearestUnitIndex];
                _destinationRequestStash.Set(unit, new DestinationRequest
                {
                    Target = unit,
                    Destination = slotPosition
                });
            }
        }

        private static int FindNearestFreeUnit(Vector3 slotPosition, List<Vector3> unitPositions, List<bool> taken)
        {
            int bestIndex = -1;
            float bestSqrDist = float.PositiveInfinity;

            for (int i = 0; i < unitPositions.Count; i++)
            {
                if (taken[i])
                    continue;

                float sqrDist = SqrDistanceXZ(unitPositions[i], slotPosition);
                if (sqrDist < bestSqrDist)
                {
                    bestSqrDist = sqrDist;
                    bestIndex = i;
                }
            }

            return bestIndex;
        }

        private static float SqrDistanceXZ(Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dz = a.z - b.z;
            return dx * dx + dz * dz;
        }

        private static float ProjectXZ(Vector3 offset, Vector3 direction)
        {
            // direction уже нормализован и лежит в плоскости XZ (y = 0)
            return offset.x * direction.x + offset.z * direction.z;
        }

        private static Vector3 ComputeCentroid(List<Vector3> points)
        {
            if (points.Count == 0)
                return Vector3.zero;

            Vector3 sum = Vector3.zero;
            for (int i = 0; i < points.Count; i++)
                sum += points[i];
            return sum / points.Count;
        }

        private void IssueHidePreview()
        {
            var hideEntity = World.CreateEntity();
            _hideRequestStash.Set(hideEntity, new HideMovePreviewRequest());
        }

        public void Dispose()
        {
        }
    }
}