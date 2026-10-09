using Features.Core.Infrastructure.Interfaces;
using Features.GameplayECS.MoveCommand.Destination.Systems;
using Features.GameplayECS.MoveCommand.Ports;
using Features.GameplayECS.MoveCommand.View;
using Scellecs.Morpeh;

namespace Features.GameplayECS.MoveCommand.Destination
{
    public static class MoveDestinationFeature
    {
        public static void Add(SystemsGroup group, IObjectPool<IMoveSlotView> pool, IMoveSlotViewFactory slotViewFactory)
        {
            group.AddSystem(new ApplyCommitMoveRequestSystem());
            group.AddSystem(new CreateMoveDestinationViewSystem(pool, slotViewFactory));
            group.AddSystem(new AssignDestinationSlotsSystem());
            group.AddSystem(new SyncMoveDestinationSlotViewSystem());
            group.AddSystem(new ShowMoveDestinationViewSystem());
            group.AddSystem(new SetUnitDestinationSystem());
            group.AddSystem(new CleanUpMoveDestinationSlotsOnArrivalSystem(pool));
            group.AddSystem(new CleanUpMoveFormationsSystem());
        }
    }
}