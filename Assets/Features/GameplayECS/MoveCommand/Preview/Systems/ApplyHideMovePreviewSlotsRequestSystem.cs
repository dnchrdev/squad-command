using System.Collections.Generic;
using Features.Core.Infrastructure.Interfaces;
using Features.GameplayECS.Common;
using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Preview.Components;
using Features.GameplayECS.MoveCommand.Preview.Requests;
using Features.GameplayECS.MoveCommand.View;
using Scellecs.Morpeh;

namespace Features.GameplayECS.MoveCommand.Preview.Systems
{
    public class ApplyHideMovePreviewSlotsRequestSystem : ISystem
    {
        public World World { get; set; }

        private readonly IObjectPool<IMoveSlotView> _pool;

        private Filter _requestFilter;
        private Filter _movePreviewFormationFilter;

        private Stash<HideMovePreviewSlotsRequest> _requestStash;
        private Stash<MovePreviewSlotTag> _movePreviewSlotStash;
        private Stash<MoveSlotAssetPath> _assetPathStash;
        private Stash<MoveSlotPosition> _positionStash;
        private Stash<MoveSlotViewShowedTag> _moveSlotViewShowedTagStash;
        private Stash<MoveSlotViewComponent> _moveSlotViewComponentStash;
        private Stash<MovePreviewFormation> _movePreviewFormationStash;
        private Stash<MoveSlotSpawningTag> _spawningTagStash;

        private readonly List<Entity> _slotsBuffer = new List<Entity>();

        public ApplyHideMovePreviewSlotsRequestSystem(IObjectPool<IMoveSlotView> pool)
        {
            _pool = pool;
        }

        public void OnAwake()
        {
            _requestFilter = World.Filter
                .With<HideMovePreviewSlotsRequest>()
                .Build();

            _movePreviewFormationFilter = World.Filter
                .With<MovePreviewFormation>()
                .Build();
            
            _requestStash = World.GetStash<HideMovePreviewSlotsRequest>();
            _movePreviewSlotStash = World.GetStash<MovePreviewSlotTag>();
            _assetPathStash = World.GetStash<MoveSlotAssetPath>();
            _positionStash = World.GetStash<MoveSlotPosition>();
            _spawningTagStash = World.GetStash<MoveSlotSpawningTag>();
            _moveSlotViewShowedTagStash = World.GetStash<MoveSlotViewShowedTag>();
            _moveSlotViewComponentStash = World.GetStash<MoveSlotViewComponent>();
            _movePreviewFormationStash = World.GetStash<MovePreviewFormation>();
        }

        public void OnUpdate(float deltaTime)
        {
            bool hasRequest = false;

            foreach (var _ in _requestFilter)
            {
                hasRequest = true;
                break;
            }

            if (hasRequest == false) return;

            foreach (var previewFormation in _movePreviewFormationFilter)
            {
                ref var previewFormationSlotsComp = ref _movePreviewFormationStash.Get(previewFormation);

                _slotsBuffer.Clear();
                _slotsBuffer.AddRange(previewFormationSlotsComp.Slots);

                foreach (var removeSlot in previewFormationSlotsComp.Slots)
                {
                    _assetPathStash.Remove(removeSlot);
                    _spawningTagStash.Remove(removeSlot);
                    _movePreviewSlotStash.Remove(removeSlot);
                    _positionStash.Remove(removeSlot);

                    if (_moveSlotViewComponentStash.Has(removeSlot))
                    {
                        ref var moveSlotView = ref _moveSlotViewComponentStash.Get(removeSlot);

                        moveSlotView.Value.Hide();
                        _pool.Return(moveSlotView.Value);
                        _moveSlotViewComponentStash.Remove(removeSlot);

                        if (_moveSlotViewShowedTagStash.Has(removeSlot))
                            _moveSlotViewShowedTagStash.Remove(removeSlot);
                    }
                }
                
                foreach (var removeSlot in _slotsBuffer)
                {
                    previewFormationSlotsComp.Slots.Remove(removeSlot);
                    World.RemoveEntity(removeSlot);
                }
            }

            RequestUtils.ConsumeAllRequests(World, _requestFilter, _requestStash);
        }

        public void Dispose()
        {
        }
    }
}