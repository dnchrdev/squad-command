using System.Collections.Generic;
using Features.GameplayECS.Common.Components;
using Features.GameplayECS.MoveCommand.Destination.Components;
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
            _destinationSlotsAssignedTagStash = World.GetStash<DestinationSlotsAssignedTag>();
            _applyDestinationFormationTagStash = World.GetStash<ApplyDestinationFormationTag>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var destinationFormation in _moveDestinationFormationFilter)
            {
                ref var destinationFormationComp = ref _destinationFormationStash.Get(destinationFormation);

                CollectWithPositions(destinationFormationComp.Units, _unitsBuffer, _unitPositionsBuffer);
                CollectWithPositions(destinationFormationComp.Slots, _slotsBuffer, _slotPositionsBuffer);

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

        private void CollectWithPositions(List<Entity> source, List<Entity> entities, List<Vector3> positions)
        {
            entities.Clear();
            positions.Clear();

            foreach (var entity in source)
            {
                if (!_positionStash.Has(entity)) continue;

                entities.Add(entity);
                positions.Add(_positionStash.Get(entity).Value);
            }
        }

        public void Dispose()
        {
        }
    }
}