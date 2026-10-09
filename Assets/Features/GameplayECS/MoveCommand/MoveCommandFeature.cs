using Features.Core.Infrastructure;
using Features.GameplayECS.MoveCommand.Destination;
using Features.GameplayECS.MoveCommand.Ports;
using Features.GameplayECS.MoveCommand.Preview;
using Features.GameplayECS.MoveCommand.View;
using Scellecs.Morpeh;

namespace Features.GameplayECS.MoveCommand
{
    public static class MoveCommandFeature
    {
        public static void Add(SystemsGroup group, IMoveSlotViewFactory slotViewFactory)
        {
            MovePreviewFeature.Add(group, new ObjectPool<IMoveSlotView>(), slotViewFactory);
            MoveDestinationFeature.Add(group, new ObjectPool<IMoveSlotView>(), slotViewFactory);
        }
    }
}