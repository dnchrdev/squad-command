using Features.GameplayECS.MoveCommand.Components;
using Scellecs.Morpeh;

namespace Features.GameplayECS.MoveCommand.Preview.Systems
{
    public class SyncMovePreviewSlotViewSystem : ISystem
    {
        public World World { get; set; }

        private Filter _syncMoveSlotFilter;

        private Stash<MoveSlotPosition> _positionStash;
        private Stash<MoveSlotViewComponent> _slotViewStash;

        public void OnAwake()
        {
            _syncMoveSlotFilter = World.Filter
                .With<MovePreviewSlotTag>()
                .With<MoveSlotPosition>()
                .With<MoveSlotViewComponent>()
                .Build();

            _positionStash = World.GetStash<MoveSlotPosition>();
            _slotViewStash = World.GetStash<MoveSlotViewComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var slot in _syncMoveSlotFilter)
            {
                ref var slotPosition = ref _positionStash.Get(slot);
                ref var slotView = ref _slotViewStash.Get(slot);

                slotView.Value.SetPosition(slotPosition.Value);
            }
        }

        public void Dispose()
        {
        }
    }
}