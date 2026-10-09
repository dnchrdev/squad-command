using System.Collections.Generic;
using Scellecs.Morpeh;

namespace Features.GameplayECS.MoveCommand.Preview.Components
{
    public struct MovePreviewFormation : IComponent
    {
        public List<Entity> Slots;
    }
}