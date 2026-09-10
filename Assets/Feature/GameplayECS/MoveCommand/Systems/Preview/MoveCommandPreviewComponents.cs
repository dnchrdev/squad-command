using System.Collections.Generic;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.MoveCommand.Systems.Preview
{
    
    public struct MovePreviewSlot : IComponent
    {
    }
    
    public struct MovePreviewFormation : IComponent
    {
        public List<Entity> Slots;
    }
    
}