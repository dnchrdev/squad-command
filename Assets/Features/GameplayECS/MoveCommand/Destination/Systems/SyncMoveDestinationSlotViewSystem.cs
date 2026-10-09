using Features.GameplayECS.Common.Components;
using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Destination.Components;
using Scellecs.Morpeh;

namespace Features.GameplayECS.MoveCommand.Destination.Systems
{
    public class SyncMoveDestinationSlotViewSystem : ISystem
    {
        public World World { get; set; }

        private Filter _syncMoveSlotFilter;

        private Stash<Position> _positionStash;
        private Stash<MoveSlotViewComponent> _slotViewStash;

        public void OnAwake()
        {
            _syncMoveSlotFilter = World.Filter
                .With<MoveDestinationSlotTag>()
                .With<Position>()
                .With<MoveSlotViewComponent>()
                .Build();

            _positionStash = World.GetStash<Position>();
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