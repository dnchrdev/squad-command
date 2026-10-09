using Features.Core.Infrastructure.Interfaces;
using Features.GameplayECS.MoveCommand.Ports;
using Features.GameplayECS.MoveCommand.Preview.Systems;
using Features.GameplayECS.MoveCommand.View;
using Scellecs.Morpeh;

namespace Features.GameplayECS.MoveCommand.Preview
{
    public static class MovePreviewFeature
    {
        public static void Add(SystemsGroup group, IObjectPool<IMoveSlotView> pool, IMoveSlotViewFactory slotViewFactory)
        {
            group.AddSystem(new ApplyHideMovePreviewSlotsRequestSystem(pool));
            group.AddSystem(new ApplyNewMovePreviewFormationRequestSystem());
            group.AddSystem(new ApplyUpdateMovePreviewFormationRequestSystem());
            group.AddSystem(new CreateMovePreviewViewSystem(pool, slotViewFactory));
            group.AddSystem(new SyncMovePreviewSlotViewSystem());
            group.AddSystem(new ShowMovePreviewSystem());
        }
    }
}