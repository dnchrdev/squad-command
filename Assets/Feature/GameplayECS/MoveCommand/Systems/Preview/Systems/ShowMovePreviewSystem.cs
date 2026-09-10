using Feature.GameplayECS.Common;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.MoveCommand.Systems.Preview.Systems
{
    public class ShowMovePreviewSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _needToShowMoveSlotViewFilter;
        
        private Stash<MoveSlotViewShowedTag> _moveSlotViewShowedTagStash;
        private Stash<MoveSlotViewComponent> _moveSlotViewStash;
        
        public void OnAwake()
        {
            _needToShowMoveSlotViewFilter = World.Filter
                .With<MovePreviewSlot>()
                .With<MoveSlotViewComponent>()
                .Without<AssetPath>()
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