using Features.GameplayECS.MoveCommand.Components;
using Features.GameplayECS.MoveCommand.Destination.Components;
using Scellecs.Morpeh;

namespace Features.GameplayECS.MoveCommand.Destination.Systems
{
    public class ShowMoveDestinationViewSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _needToShowMoveSlotViewFilter;
        
        private Stash<MoveSlotViewShowedTag> _moveSlotViewShowedTagStash;
        private Stash<MoveSlotViewComponent> _moveSlotViewStash;
        
        public void OnAwake()
        {
            _needToShowMoveSlotViewFilter = World.Filter
                .With<MoveDestinationSlotTag>()
                .With<MoveSlotViewComponent>()
                .Without<MoveSlotAssetPath>()
                .Without<MoveSlotViewShowedTag>()
                .Build();
        
            _moveSlotViewShowedTagStash = World.GetStash<MoveSlotViewShowedTag>();
            _moveSlotViewStash = World.GetStash<MoveSlotViewComponent>();
        }
        
        public void OnUpdate(float deltaTime)
        {
            foreach (var slot in _needToShowMoveSlotViewFilter)
            {
                ref var view = ref _moveSlotViewStash.Get(slot);
        
                view.Value.Show();
                _moveSlotViewShowedTagStash.Set(slot);
            }
        }
        
        public void Dispose()
        {
        }
    }
}