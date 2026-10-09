using System.Collections.Generic;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Selection.Requests
{
    public enum SelectionRequestType
    {
        PreviewAdd,
        PreviewReplace,
        PreviewRemove,
        ClearSelection,
        ClearPreview,
        Commit
    }

    public struct SelectionRequest : IComponent
    {
        public SelectionRequestType Type;
        public List<Entity> Units;
        public int Sequence;
    }
}