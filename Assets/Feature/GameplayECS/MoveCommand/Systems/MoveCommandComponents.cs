using Feature.GameplayECS.MoveCommand.Adapter;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.MoveCommand.Systems
{
    public struct MoveSlotViewComponent : IComponent
    {
        public IMoveSlotView Value;
    }

    public struct MoveSlotViewShowedTag : IComponent
    {
    }
}