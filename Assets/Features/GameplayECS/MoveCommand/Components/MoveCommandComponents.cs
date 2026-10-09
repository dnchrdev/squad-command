using Features.GameplayECS.MoveCommand.View;
using Scellecs.Morpeh;

namespace Features.GameplayECS.MoveCommand.Components
{
    public struct MoveSlotViewComponent : IComponent { public IMoveSlotView Value; }
    public struct MoveSlotViewShowedTag : IComponent { }
    public struct MovePreviewSlotTag : IComponent { }
}