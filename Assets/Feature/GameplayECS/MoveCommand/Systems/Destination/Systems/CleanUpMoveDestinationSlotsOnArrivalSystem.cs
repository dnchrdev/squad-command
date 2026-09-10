using System.Collections.Generic;
using Feature.Core.Infrastructure.Interfaces;
using Feature.GameplayECS.Common;
using Feature.GameplayECS.MoveCommand.Adapter;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.Systems.Destination.Systems
{
    public class CleanUpMoveDestinationSlotsOnArrivalSystem : ISystem
    {
        private const float SLOT_HIDE_DISTANCE = 0.25f;

        private readonly IObjectPool<IMoveSlotView> _pool;

        public World World { get; set; }

        private Filter _activeMoveFormationsFilter;

        private Stash<MoveDestinationFormation> _destinationFormationStash;
        private Stash<MoveDestinationSlotTag> _moveDestinationSlotStash;
        private Stash<UnitMoveDestinationSlot> _unitMoveDestinationSlotStash;
        private Stash<Position> _positionStash;
        private Stash<MoveSlotViewComponent> _moveSlotViewStash;
        private Stash<MoveSlotViewShowedTag> _moveSlotViewShowedTagStash;
        private Stash<AssetPath> _assetPathStash;
        private Stash<Common.SpawningTag> _spawningTagStash;

        private readonly List<Entity> _unitsBuffer = new();

        public CleanUpMoveDestinationSlotsOnArrivalSystem(IObjectPool<IMoveSlotView> pool)
        {
            _pool = pool;
        }

        public void OnAwake()
        {
            _activeMoveFormationsFilter = World.Filter
                .With<MoveDestinationFormation>()
                .With<DestinationSlotsAssignedTag>()
                .Without<ApplyDestinationFormationSelfRequest>()
                .Build();

            _destinationFormationStash = World.GetStash<MoveDestinationFormation>();
            _moveDestinationSlotStash = World.GetStash<MoveDestinationSlotTag>();
            _unitMoveDestinationSlotStash = World.GetStash<UnitMoveDestinationSlot>();
            _positionStash = World.GetStash<Position>();
            _moveSlotViewStash = World.GetStash<MoveSlotViewComponent>();
            _moveSlotViewShowedTagStash = World.GetStash<MoveSlotViewShowedTag>();
            _assetPathStash = World.GetStash<AssetPath>();
            _spawningTagStash = World.GetStash<SpawningTag>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var formation in _activeMoveFormationsFilter)
            {
                ref var destinationFormationComp = ref _destinationFormationStash.Get(formation);

                _unitsBuffer.Clear();

                foreach (var unit in destinationFormationComp.Units)
                {
                    // if (_unitMoveDestinationSlotStash.Has(unit))
                    {
                        ref var destinationSlotComp = ref _unitMoveDestinationSlotStash.Get(unit);
                        ref var slotPositionComp = ref _positionStash.Get(destinationSlotComp.Slot);
                        ref var unitPosition = ref _positionStash.Get(unit);

                        Vector3 deltaDestination = slotPositionComp.Value - unitPosition.Value;
                        Vector3 projectedDeltaDestination = Vector3.ProjectOnPlane(deltaDestination, Vector3.up);

                        if (projectedDeltaDestination.magnitude <= SLOT_HIDE_DISTANCE)
                        {
                            if (_moveSlotViewStash.Has(destinationSlotComp.Slot))
                            {
                                ref var moveSlotViewComp = ref _moveSlotViewStash.Get(destinationSlotComp.Slot);

                                moveSlotViewComp.Value.Hide();
                                _pool.Return(moveSlotViewComp.Value);
                                _moveSlotViewStash.Remove(destinationSlotComp.Slot);

                                if (_moveSlotViewShowedTagStash.Has(destinationSlotComp.Slot))
                                    _moveSlotViewShowedTagStash.Remove(destinationSlotComp.Slot);
                            }

                            _assetPathStash.Remove(destinationSlotComp.Slot);
                            _spawningTagStash.Remove(destinationSlotComp.Slot);

                            destinationFormationComp.Slots.Remove(destinationSlotComp.Slot);
                            _moveDestinationSlotStash.Remove(destinationSlotComp.Slot);
                            _positionStash.Remove(destinationSlotComp.Slot);
                            _unitsBuffer.Add(unit);
                            World.RemoveEntity(destinationSlotComp.Slot);
                        }
                    }
                }

                foreach (var unit in _unitsBuffer)
                {
                    _unitMoveDestinationSlotStash.Remove(unit);
                    destinationFormationComp.Units.Remove(unit);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}