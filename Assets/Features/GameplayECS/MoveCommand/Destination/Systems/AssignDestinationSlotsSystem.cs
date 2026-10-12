using System.Collections.Generic;
using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Destination.Components;
using Features.GameplayECS.Synchronization.Outputs;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.MoveCommand.Destination.Systems
{
    public sealed class AssignDestinationSlotsSystem : ISystem
    {
        public World World { get; set; }

        private Filter _moveDestinationFormationFilter;

        private Stash<MoveDestinationFormation> _destinationFormationStash;
        private Stash<UnitMoveDestinationSlot> _unitMoveDestinationSlotStash;
        private Stash<Position> _positionStash;
        private Stash<MoveSlotPosition> _slotPositionStash;
        private Stash<DestinationSlotsAssignedTag> _destinationSlotsAssignedTagStash;
        private Stash<ApplyDestinationFormationTag> _applyDestinationFormationTagStash;

        private readonly SlotAssignmentSolver _solver = new();
        private readonly List<Entity> _unitsBuffer = new();
        private readonly List<Vector3> _unitPositionsBuffer = new();
        private readonly List<Entity> _slotsBuffer = new();
        private readonly List<Vector3> _slotPositionsBuffer = new();
        private readonly List<int> _slotIndexOfUnitBuffer = new();

        public void OnAwake()
        {
            _moveDestinationFormationFilter = World.Filter
                .With<MoveDestinationFormation>()
                .Without<DestinationSlotsAssignedTag>()
                .Build();

            _destinationFormationStash = World.GetStash<MoveDestinationFormation>();
            _unitMoveDestinationSlotStash = World.GetStash<UnitMoveDestinationSlot>();
            _positionStash = World.GetStash<Position>();
            _slotPositionStash = World.GetStash<MoveSlotPosition>();
            _destinationSlotsAssignedTagStash = World.GetStash<DestinationSlotsAssignedTag>();
            _applyDestinationFormationTagStash = World.GetStash<ApplyDestinationFormationTag>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var destinationFormation in _moveDestinationFormationFilter)
            {
                ref var destinationFormationComp = ref _destinationFormationStash.Get(destinationFormation);

                CollectUnitsWithPositions(destinationFormationComp.Units);
                CollectSlotsWithPositions(destinationFormationComp.Slots);

                _solver.Solve(_unitPositionsBuffer, _slotPositionsBuffer, _slotIndexOfUnitBuffer);

                for (int i = 0; i < _unitsBuffer.Count; i++)
                {
                    int slotIndex = _slotIndexOfUnitBuffer[i];
                    if (slotIndex < 0) continue;

                    _unitMoveDestinationSlotStash.Set(_unitsBuffer[i], new UnitMoveDestinationSlot
                    {
                        Slot = _slotsBuffer[slotIndex],
                        OwnedFormation = destinationFormation
                    });
                }

                _applyDestinationFormationTagStash.Set(destinationFormation);
                _destinationSlotsAssignedTagStash.Set(destinationFormation);
            }
        }

        private void CollectUnitsWithPositions(List<Entity> units)
        {
            _unitsBuffer.Clear();
            _unitPositionsBuffer.Clear();

            foreach (var unit in units)
            {
                if (!_positionStash.Has(unit)) continue;

                _unitsBuffer.Add(unit);
                _unitPositionsBuffer.Add(_positionStash.Get(unit).Value);
            }
        }

        private void CollectSlotsWithPositions(List<Entity> slots)
        {
            _slotsBuffer.Clear();
            _slotPositionsBuffer.Clear();

            foreach (var slot in slots)
            {
                if (!_slotPositionStash.Has(slot)) continue;

                _slotsBuffer.Add(slot);
                _slotPositionsBuffer.Add(_slotPositionStash.Get(slot).Value);
            }
        }

        public void Dispose()
        {
        }
    }
}