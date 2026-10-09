using System.Collections.Generic;
using Scellecs.Morpeh;

namespace Features.Shared.Units
{
    public interface ISelectionCommands
    {
        void AddToSelectionPreview(IReadOnlyList<Entity> units);
        void ReplaceSelectionPreview(IReadOnlyList<Entity> units);
        void RemoveFromSelectionPreview(IReadOnlyList<Entity> units);
        void ClearSelection();
        void ClearSelectionPreview();
        void CommitSelection();
    }
}