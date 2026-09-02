using System.Collections.Generic;
using Feature.GameplayECS.Common;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.AI;

namespace Feature.GameplayECS.Navigation.Systems
{
    public class SquadFormationSystem : ISystem
    {
        public World World { get; set; }

        private Filter _newOrderFilter;
        private Filter _activeOrderFilter;

        private Stash<MoveOrderRequest> _moveOrderRequestStash;
        private Stash<SquadFormationOrder> _formationOrderStash;
        private Stash<SquadSlotClaim> _slotClaimStash;
        private Stash<Position> _positionStash;
        private Stash<UnitRadius> _radiusStash;
        private Stash<DestinationRequest> _destinationRequestStash;

        private const float DefaultSpacing = 1.2f;
        private const float SlotArrivalTolerance = 0.35f;

        private NavMeshPath _directionPath;

        private readonly List<Entity> _aliveUnitsBuffer = new List<Entity>(64);
        private readonly List<Vector3> _slotsBuffer = new List<Vector3>(64);

        public void OnAwake()
        {
            _newOrderFilter = World.Filter
                .With<MoveOrderRequest>()
                .Build();

            _activeOrderFilter = World.Filter
                .With<SquadFormationOrder>()
                .Build();

            _moveOrderRequestStash = World.GetStash<MoveOrderRequest>();
            _formationOrderStash = World.GetStash<SquadFormationOrder>();
            _slotClaimStash = World.GetStash<SquadSlotClaim>();
            _positionStash = World.GetStash<Position>();
            _radiusStash = World.GetStash<UnitRadius>();
            _destinationRequestStash = World.GetStash<DestinationRequest>();

            _directionPath = new NavMeshPath();
        }

        public void OnUpdate(float deltaTime)
        {
            CreateOrdersFromRequests();
            AdvanceActiveOrders();
        }

        private void CreateOrdersFromRequests()
        {
            foreach (var requestEntity in _newOrderFilter)
            {
                ref var request = ref _moveOrderRequestStash.Get(requestEntity);
                var rawUnits = request.Units;

                _aliveUnitsBuffer.Clear();
                if (rawUnits != null)
                {
                    for (int i = 0; i < rawUnits.Count; i++)
                    {
                        var unit = rawUnits[i];
                        if (_positionStash.Has(unit))
                            _aliveUnitsBuffer.Add(unit);
                    }
                }

                if (_aliveUnitsBuffer.Count > 0)
                {
                    for (int i = 0; i < _aliveUnitsBuffer.Count; i++)
                        DetachFromPreviousClaim(_aliveUnitsBuffer[i]);

                    // Если MoveOrderRequest пришёл с явными слотами (например, из
                    // CommitMoveSystem — позициями, которые игрок уже видел в превью),
                    // используем их напрямую вместо генерации кольца вокруг center.
                    // Fallback на GenerateRingSlots остаётся для вызовов без явных слотов
                    // (например, приказ на движение без предварительного превью).
                    List<Vector3> explicitSlots = request.TargetPoints;
                    bool hasExplicitSlots = explicitSlots != null && explicitSlots.Count > 0;

                    CreateFormationOrder(_aliveUnitsBuffer, Vector3.zero, hasExplicitSlots ? explicitSlots : null);
                }

                _moveOrderRequestStash.Remove(requestEntity);
                World.RemoveEntity(requestEntity);
            }
        }

        private void DetachFromPreviousClaim(Entity unit)
        {
            if (!_slotClaimStash.Has(unit))
                return;

            ref var claim = ref _slotClaimStash.Get(unit);
            var oldOrderEntity = claim.OrderEntity;
            _slotClaimStash.Remove(unit);

            if (!_formationOrderStash.Has(oldOrderEntity))
                return;

            ref var oldOrder = ref _formationOrderStash.Get(oldOrderEntity);
            oldOrder.PendingUnits?.Remove(unit);
        }

        /// <summary>
        /// Создаёт новый приказ формации.
        /// Если explicitSlots задан (не null и не пуст) — использует его как готовую
        /// раскладку слотов "как есть" (например, слоты, показанные превью-системой),
        /// а center пересчитывает как центроид этих слотов. Если explicitSlots == null —
        /// генерирует кольцевую раскладку вокруг переданного center, как раньше.
        /// </summary>
        private void CreateFormationOrder(List<Entity> units, Vector3 center, List<Vector3> explicitSlots)
        {
            if (explicitSlots != null)
            {
                _slotsBuffer.Clear();
                _slotsBuffer.AddRange(explicitSlots);

                // Явные слоты уже расположены вокруг какого-то реального центра
                // (превью строилось вокруг центра прямоугольника выделения) —
                // пересчитываем center как среднее слотов, чтобы ApproachDirection
                // считался корректно, а не от Vector3.zero.
                center = ComputeCentroidOfPoints(_slotsBuffer);
            }
            else
            {
                GenerateRingSlots(units.Count, GetSpacing(units), center, _slotsBuffer);
            }

            var orderEntity = World.CreateEntity();

            var pendingUnits = new List<Entity>(units);
            var remainingSlots = new List<Vector3>(_slotsBuffer);

            Vector3 initialCentroid = ComputeCentroid(pendingUnits);
            Vector3 approachDirection = ComputeApproachDirection(initialCentroid, center);

            _formationOrderStash.Set(orderEntity, new SquadFormationOrder
            {
                Center = center,
                PendingUnits = pendingUnits,
                RemainingSlots = remainingSlots,
                HasTarget = false,
                ApproachDirection = approachDirection
            });

            for (int i = 0; i < pendingUnits.Count; i++)
            {
                _slotClaimStash.Set(pendingUnits[i], new SquadSlotClaim
                {
                    OrderEntity = orderEntity
                });
            }
        }

        private void AdvanceActiveOrders()
        {
            foreach (var orderEntity in _activeOrderFilter)
            {
                ref var order = ref _formationOrderStash.Get(orderEntity);

                RemoveDeadUnits(order.PendingUnits);

                if (order.PendingUnits.Count == 0)
                {
                    _formationOrderStash.Remove(orderEntity);
                    World.RemoveEntity(orderEntity);
                    continue;
                }

                bool targetChanged = false;

                if (order.HasTarget)
                {
                    int arrivedIndex = FindArrivedUnit(order.PendingUnits, order.CurrentTargetSlot);
                    if (arrivedIndex >= 0)
                    {
                        var arrivedUnit = order.PendingUnits[arrivedIndex];

                        order.PendingUnits.RemoveAt(arrivedIndex);
                        order.RemainingSlots.Remove(order.CurrentTargetSlot);
                        _slotClaimStash.Remove(arrivedUnit);
                        order.HasTarget = false;
                        targetChanged = true;

                        if (order.PendingUnits.Count == 0)
                        {
                            _formationOrderStash.Remove(orderEntity);
                            World.RemoveEntity(orderEntity);
                            continue;
                        }
                    }
                }

                if (!order.HasTarget)
                {
                    if (!TryFindSlotFarthestAlongDirection(order.RemainingSlots, order.Center, order.ApproachDirection,
                            out Vector3 farthestSlot))
                    {
                        farthestSlot = order.Center;
                    }

                    order.CurrentTargetSlot = farthestSlot;
                    order.HasTarget = true;
                    targetChanged = true;
                }

                if (targetChanged)
                {
                    for (int i = 0; i < order.PendingUnits.Count; i++)
                    {
                        _destinationRequestStash.Set(order.PendingUnits[i], new DestinationRequest
                        {
                            Target = order.PendingUnits[i],
                            Destination = order.CurrentTargetSlot
                        });
                    }
                }
            }
        }

        private void RemoveDeadUnits(List<Entity> units)
        {
            for (int i = units.Count - 1; i >= 0; i--)
            {
                if (!_positionStash.Has(units[i]))
                    units.RemoveAt(i);
            }
        }

        private int FindArrivedUnit(List<Entity> units, Vector3 targetSlot)
        {
            float sqrTolerance = SlotArrivalTolerance * SlotArrivalTolerance;
            for (int i = 0; i < units.Count; i++)
            {
                ref var position = ref _positionStash.Get(units[i]);
                if ((position.Value - targetSlot).sqrMagnitude <= sqrTolerance)
                    return i;
            }

            return -1;
        }

        private Vector3 ComputeCentroid(List<Entity> units)
        {
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < units.Count; i++)
            {
                ref var position = ref _positionStash.Get(units[i]);
                sum += position.Value;
            }

            return sum / units.Count;
        }

        private static Vector3 ComputeCentroidOfPoints(List<Vector3> points)
        {
            if (points.Count == 0)
                return Vector3.zero;

            Vector3 sum = Vector3.zero;
            for (int i = 0; i < points.Count; i++)
                sum += points[i];
            return sum / points.Count;
        }

        private Vector3 ComputeApproachDirection(Vector3 from, Vector3 to)
        {
            bool success = NavMesh.CalculatePath(from, to, NavMesh.AllAreas, _directionPath);

            if (success && _directionPath.corners.Length >= 2)
            {
                var corners = _directionPath.corners;
                Vector3 last = corners[corners.Length - 1];
                Vector3 prev = corners[corners.Length - 2];
                Vector3 dir = last - prev;
                dir.y = 0f;

                if (dir.sqrMagnitude > 0.0001f)
                    return dir.normalized;
            }

            Vector3 straight = to - from;
            straight.y = 0f;

            if (straight.sqrMagnitude > 0.0001f)
                return straight.normalized;

            return Vector3.forward;
        }

        private bool TryFindSlotFarthestAlongDirection(List<Vector3> slots, Vector3 fromPoint,
            Vector3 approachDirection, out Vector3 result)
        {
            result = default;
            if (slots.Count == 0)
                return false;

            float bestProjection = float.NegativeInfinity;
            int bestIndex = -1;
            for (int i = 0; i < slots.Count; i++)
            {
                Vector3 offset = slots[i] - fromPoint;
                offset.y = 0f;

                float projection = Vector3.Dot(offset, approachDirection);
                if (projection > bestProjection)
                {
                    bestProjection = projection;
                    bestIndex = i;
                }
            }

            result = slots[bestIndex];
            return true;
        }

        private float GetSpacing(List<Entity> units)
        {
            float sum = 0f;
            int count = 0;
            for (int i = 0; i < units.Count; i++)
            {
                if (_radiusStash.Has(units[i]))
                {
                    sum += _radiusStash.Get(units[i]).Value;
                    count++;
                }
            }

            if (count == 0) return DefaultSpacing;
            return Mathf.Max(DefaultSpacing, (sum / count) * 2.2f);
        }

        private void GenerateRingSlots(int count, float spacing, Vector3 center, List<Vector3> result)
        {
            result.Clear();
            result.Add(center);

            int ring = 1;
            while (result.Count < count)
            {
                int pointsInRing = 6 * ring;
                float radius = spacing * ring;

                for (int i = 0; i < pointsInRing && result.Count < count; i++)
                {
                    float angle = (2f * Mathf.PI / pointsInRing) * i;
                    Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
                    result.Add(center + offset);
                }

                ring++;
            }
        }

        public void Dispose()
        {
        }
    }
}