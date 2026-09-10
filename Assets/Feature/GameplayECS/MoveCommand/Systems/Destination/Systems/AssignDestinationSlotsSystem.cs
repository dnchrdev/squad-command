using System.Collections.Generic;
using Feature.GameplayECS.Common;
using Feature.GameplayECS.SelectCommand;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.Systems.Destination.Systems
{
    public class AssignDestinationSlotsSystem : ISystem
    {
        public World World { get; set; }

        private Filter _moveDestinationFormationFilter;
        private Filter _selectedUnitsFilter;
        
        private Stash<MoveDestinationFormation> _destinationFormationStash;
        private Stash<UnitMoveDestinationSlot> _unitMoveDestinationSlotStash;
        private Stash<Position> _positionStash;
        private Stash<DestinationSlotsAssignedTag> _destinationSlotsAssignedTagStash;
        private Stash<ApplyDestinationFormationSelfRequest> _appplyDestinationSelfRequestStash;

        private readonly List<Entity> _freeSlotsBuffer = new ();
        private readonly List<Entity> _availableUnitsBuffer = new ();

        public void OnAwake()
        {
            _moveDestinationFormationFilter = World.Filter
                .With<MoveDestinationFormation>()
                .Without<DestinationSlotsAssignedTag>()
                .Build();

            _selectedUnitsFilter = World.Filter
                .With<Selected>()
                .With<Position>()
                .Build();

            _destinationFormationStash = World.GetStash<MoveDestinationFormation>();
            _unitMoveDestinationSlotStash = World.GetStash<UnitMoveDestinationSlot>();
            _positionStash = World.GetStash<Position>();
            _destinationSlotsAssignedTagStash = World.GetStash<DestinationSlotsAssignedTag>();
            _appplyDestinationSelfRequestStash = World.GetStash<ApplyDestinationFormationSelfRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var destinationFormation in _moveDestinationFormationFilter)
            {
                ref var destinationFormationComp = ref _destinationFormationStash.Get(destinationFormation);

                _freeSlotsBuffer.Clear();
                _freeSlotsBuffer.AddRange(destinationFormationComp.Slots);

                _availableUnitsBuffer.Clear();
                _availableUnitsBuffer.AddRange(destinationFormationComp.Units);

                Vector3 direction = new Vector3(
                    destinationFormationComp.OrderDirection.x,
                    0f,
                    destinationFormationComp.OrderDirection.y);

                direction = direction.sqrMagnitude < 1e-6f ? Vector3.forward : direction.normalized;
                Vector3 squadOrigin = ComputeSquadOrigin();

                while (true)
                {
                    bool hasFreeSlot =
                        TryFindFarthestFreeSlot(squadOrigin, direction, out var slotEntity, out var slotPosition);

                    Entity unit = default;

                    bool hasAvailableUnit = hasFreeSlot && TryFindNearestAvailableUnit(slotPosition, out unit);

                    if (hasFreeSlot && hasAvailableUnit)
                    {
                        _unitMoveDestinationSlotStash.Set(unit, new UnitMoveDestinationSlot
                        {
                            Slot = slotEntity,
                            OwnedFormation = destinationFormation
                        });

                        _freeSlotsBuffer.Remove(slotEntity);
                        _availableUnitsBuffer.Remove(unit);
                    }
                    else break;
                }

                _appplyDestinationSelfRequestStash.Set(destinationFormation);
                _destinationSlotsAssignedTagStash.Set(destinationFormation);
            }
        }

        private Vector3 ComputeSquadOrigin()
        {
            Vector3 sum = Vector3.zero;
            int count = 0;
            foreach (var unit in _selectedUnitsFilter)
            {
                if (!_positionStash.Has(unit)) continue;
                sum += _positionStash.Get(unit).Value;
                count++;
            }

            return count > 0 ? sum / count : Vector3.zero;
        }

        private bool TryFindFarthestFreeSlot(Vector3 origin, Vector3 direction, out Entity result,
            out Vector3 resultPosition)
        {
            result = default;
            resultPosition = default;
            float best = float.NegativeInfinity;
            bool found = false;
            foreach (var slotEntity in _freeSlotsBuffer)
            {
                if (!_positionStash.Has(slotEntity)) continue;
                var pos = _positionStash.Get(slotEntity).Value;
                float projection = ProjectXZ(pos - origin, direction);
                if (projection > best)
                {
                    best = projection;
                    result = slotEntity;
                    resultPosition = pos;
                    found = true;
                }
            }

            return found;
        }

        private bool TryFindNearestAvailableUnit(Vector3 slotPosition, out Entity result)
        {
            result = default;
            float bestSqrDist = float.PositiveInfinity;
            bool found = false;
            foreach (var unit in _availableUnitsBuffer)
            {
                if (!_positionStash.Has(unit)) continue;
                var unitPos = _positionStash.Get(unit).Value;
                float dx = unitPos.x - slotPosition.x, dz = unitPos.z - slotPosition.z;
                float sqrDist = dx * dx + dz * dz;
                if (sqrDist < bestSqrDist)
                {
                    bestSqrDist = sqrDist;
                    result = unit;
                    found = true;
                }
            }

            return found;
        }

        private static float ProjectXZ(Vector3 offset, Vector3 direction) =>
            offset.x * direction.x + offset.z * direction.z;

        public void Dispose()
        {
        }
    }
}