using System.Collections.Generic;
using Feature.GameplayECS.Common;
using Feature.GameplayECS.SelectCommand;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.MoveCommand.Systems.Preview.Systems
{
    public class ApplyNewMovePreviewFormationRequestSystem : ISystem
    {
        private const string MOVE_SLOT_ASSET_PATH = "MoveSlotPreview";
        public World World { get; set; }
        
        private Filter _newMoveFormationRequestFilter;
        private Filter _movePreviewFormationTagFilter;
        private Filter _selectedFilter;

        private Stash<NewMovePreviewFormationRequest> _newMoveFormationRequestStash;
        private Stash<MovePreviewSlot> _movePreviewSlotStash;
        private Stash<MovePreviewFormation> _movePreviewFormationStash;
        private Stash<AssetPath> _assetPathStash;

        private readonly List<Entity> _slotsBuffer = new();

        public void OnAwake()
        {
            _newMoveFormationRequestFilter = World.Filter
                .With<NewMovePreviewFormationRequest>().Build();

            _movePreviewFormationTagFilter = World.Filter
                .With<MovePreviewFormation>()
                .Build();

            _selectedFilter = World.Filter.With<Selected>().Build();

            _newMoveFormationRequestStash = World.GetStash<NewMovePreviewFormationRequest>();
            _movePreviewSlotStash = World.GetStash<MovePreviewSlot>();
            _movePreviewFormationStash = World.GetStash<MovePreviewFormation>();
            _assetPathStash = World.GetStash<AssetPath>();
        }

        public void OnUpdate(float deltaTime)
        {
            bool hasNewFormationRequest = false;

            foreach (var _ in _newMoveFormationRequestFilter)
            {
                hasNewFormationRequest = true;
                break;
            }

            if (hasNewFormationRequest == false) return;

            _slotsBuffer.Clear();

            bool isNewMovePreview = false;
            bool isPreviewFormationExist = false;
            Entity previewFormation = World.CreateEntity();

            foreach (var existedPreviewFormation in _movePreviewFormationTagFilter)
            {
                isPreviewFormationExist = true;
                World.RemoveEntity(previewFormation);
                previewFormation = existedPreviewFormation;
                break;
            }

            foreach (var unit in _selectedFilter)
            {
                isNewMovePreview = true;
                
                Entity previewSlot;
                {
                    previewSlot = World.CreateEntity();
                    _assetPathStash.Set(previewSlot, new AssetPath { Value = MOVE_SLOT_ASSET_PATH });
                }

                _movePreviewSlotStash.Set(previewSlot, new MovePreviewSlot { });

                _slotsBuffer.Add(previewSlot);
            }

            if (isNewMovePreview)
            {
                if (isPreviewFormationExist)
                {
                    ref var movePreviewFormationSlots = ref _movePreviewFormationStash.Get(previewFormation);

                    movePreviewFormationSlots.Slots.Clear();
                    movePreviewFormationSlots.Slots.AddRange(_slotsBuffer);
                    
                }
                else
                {
                    _movePreviewFormationStash.Set(previewFormation, new MovePreviewFormation
                    {
                        Slots = new List<Entity>(_slotsBuffer),
                    });
                    
                }
            }

            RequestUtils.ConsumeAllRequests(World, _newMoveFormationRequestFilter, _newMoveFormationRequestStash);
        }

        public void Dispose()
        {
        }
    }
}