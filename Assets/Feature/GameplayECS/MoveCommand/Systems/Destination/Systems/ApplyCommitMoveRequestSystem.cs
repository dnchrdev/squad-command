using System.Collections.Generic;
using Feature.GameplayECS.Common;
using Feature.GameplayECS.MoveCommand.Systems.Preview;
using Feature.GameplayECS.SelectCommand;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.MoveCommand.Systems.Destination.Systems
{
    public class ApplyCommitMoveRequestSystem : ISystem
    {
        private const string MOVE_SLOT_ASSET_PATH = "MoveSlotDestination";
        public World World { get; set; }

        private Filter _commitRequestFilter;
        private Filter _selectedFilter;
        private Filter _movePreviewSlotFilter;

        private Stash<CommitMoveRequest> _commitRequestStash;
        private Stash<MoveDestinationFormation> _moveDestinationFormationStash;
        private Stash<MoveDestinationSlotTag> _moveDestinationSlotStash;
        private Stash<UnitMoveDestinationSlot> _unitMoveDestinationSlotStash;
        private Stash<AssetPath> _assetPathStash;
        private Stash<Position> _positionStash;
        private Stash<HideMovePreviewSlotsRequest> _hideMovePreviewSlotsStash;

        private readonly Queue<Vector3> _slotsPositionsQueue = new Queue<Vector3>();
        private readonly List<Entity> _slotBuffer = new();
        private readonly List<Entity> _unitsBuffer = new();

        public void OnAwake()
        {
            _commitRequestFilter = World.Filter.With<CommitMoveRequest>().Build();
            _commitRequestStash = World.GetStash<CommitMoveRequest>();

            _selectedFilter = World.Filter
                .With<Selected>()
                .Build();

            _movePreviewSlotFilter = World.Filter.
                With<MovePreviewSlot>().
                With<Position>().
                Build();
            
            _moveDestinationFormationStash = World.GetStash<MoveDestinationFormation>();
            _unitMoveDestinationSlotStash = World.GetStash<UnitMoveDestinationSlot>();
            _assetPathStash = World.GetStash<AssetPath>();
            _positionStash = World.GetStash<Position>();
            _moveDestinationSlotStash = World.GetStash<MoveDestinationSlotTag>();
            _hideMovePreviewSlotsStash = World.GetStash<HideMovePreviewSlotsRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            bool hasCommitRequest = false;
            CommitMoveRequest requestComp = new CommitMoveRequest();

            foreach (var request in _commitRequestFilter)
            {
                hasCommitRequest = true;
                requestComp = _commitRequestStash.Get(request);
            }

            if (hasCommitRequest == false) return;

            _slotBuffer.Clear();
            _unitsBuffer.Clear();
            _slotsPositionsQueue.Clear();

            bool hasUnits = false;
            Entity destinationFormation = World.CreateEntity();

            foreach (var previewSlot in _movePreviewSlotFilter)
            {
                ref var slotPositionComp = ref _positionStash.Get(previewSlot);
                _slotsPositionsQueue.Enqueue(slotPositionComp.Value);
            }
            
            foreach (var selectedUnit in _selectedFilter)
            {
                hasUnits = true;
                bool unitHasDestinationSlot = _unitMoveDestinationSlotStash.Has(selectedUnit);

                Entity destinationSlot;

                if (unitHasDestinationSlot)
                {
                    ref var unitDestinationSlotComp = ref _unitMoveDestinationSlotStash.Get(selectedUnit);
                    destinationSlot = unitDestinationSlotComp.Slot;
                    Entity ownedFormation = unitDestinationSlotComp.OwnedFormation;
                    _unitMoveDestinationSlotStash.Remove(selectedUnit);

                    if (World.IsDisposed(ownedFormation) == false)
                    {
                        ref var previousDestinationFormationComp =
                            ref _moveDestinationFormationStash.Get(ownedFormation);

                        previousDestinationFormationComp.Slots.Remove(destinationSlot);
                        previousDestinationFormationComp.Units.Remove(selectedUnit);
                    }
                    
                }
                else
                {
                    destinationSlot = World.CreateEntity();

                    _assetPathStash.Set(destinationSlot, new AssetPath { Value = MOVE_SLOT_ASSET_PATH });
                }
                
                _positionStash.Set(destinationSlot, new Position { Value = _slotsPositionsQueue.Dequeue()});

                _moveDestinationSlotStash.Set(destinationSlot);

                _slotBuffer.Add(destinationSlot);
                _unitsBuffer.Add(selectedUnit);
            }

            if (hasUnits)
            {
                _moveDestinationFormationStash.Set(destinationFormation, new MoveDestinationFormation
                {
                    Slots = new List<Entity>(_slotBuffer),
                    Units = new List<Entity>(_unitsBuffer),
                    OrderDirection = requestComp.OrderDirection
                });
            }
            else
            {
                World.RemoveEntity(destinationFormation);
            }

            RequestUtils.ConsumeAllRequests(World, _commitRequestFilter, _commitRequestStash);

            Entity hidePreviewRequest = World.CreateEntity();
            _hideMovePreviewSlotsStash.Set(hidePreviewRequest);
        }


        public void Dispose()
        {
        }
    }
}