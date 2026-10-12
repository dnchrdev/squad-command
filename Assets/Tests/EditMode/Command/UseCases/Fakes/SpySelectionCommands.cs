using System.Collections.Generic;
using Features.Shared.Units;
using Scellecs.Morpeh;

namespace Tests.EditMode.Command.UseCases.Fakes
{
    public sealed class SpySelectionCommands : ISelectionCommands
    {
        public readonly List<string> Calls = new();

        public void AddToSelectionPreview(IReadOnlyList<Entity> units) => Calls.Add(nameof(AddToSelectionPreview));
        public void ReplaceSelectionPreview(IReadOnlyList<Entity> units) => Calls.Add(nameof(ReplaceSelectionPreview));
        public void RemoveFromSelectionPreview(IReadOnlyList<Entity> units) => Calls.Add(nameof(RemoveFromSelectionPreview));
        public void ClearSelection() => Calls.Add(nameof(ClearSelection));
        public void ClearSelectionPreview() => Calls.Add(nameof(ClearSelectionPreview));
        public void CommitSelection() => Calls.Add(nameof(CommitSelection));
    }
}