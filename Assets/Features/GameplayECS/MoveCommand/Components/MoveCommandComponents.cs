using Features.GameplayECS.MoveCommand.View;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GameplayECS.MoveCommand.Components
{
    public struct MoveSlotViewComponent : IComponent { public IMoveSlotView Value; }
    public struct MoveSlotViewShowedTag : IComponent { }
    public struct MovePreviewSlotTag : IComponent { }
    public struct MoveSlotPosition : IComponent { public Vector3 Value; }
    public struct MoveSlotAssetPath : IComponent { public string Value; }
    public struct MoveSlotSpawningTag : IComponent { }
}
