using System.Collections.Generic;
using Features.GameplayECS.Selection.Requests;
using Features.Shared.Units;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Facade
{
    public sealed class SelectionCommandsGateway : ISelectionCommands
    {
        private readonly World _world;
        private readonly Stash<SelectionRequest> _requestStash;
        private readonly List<Entity> _addBuffer = new();
        private readonly List<Entity> _replaceBuffer = new();
        private readonly List<Entity> _removeBuffer = new();
        private int _sequence;

        public SelectionCommandsGateway(World world)
        {
            _world = world;
            _requestStash = world.GetStash<SelectionRequest>();
        }

        public void AddToSelectionPreview(IReadOnlyList<Entity> units) =>
            Send(SelectionRequestType.PreviewAdd, Fill(_addBuffer, units));

        public void ReplaceSelectionPreview(IReadOnlyList<Entity> units) =>
            Send(SelectionRequestType.PreviewReplace, Fill(_replaceBuffer, units));

        public void RemoveFromSelectionPreview(IReadOnlyList<Entity> units) =>
            Send(SelectionRequestType.PreviewRemove, Fill(_removeBuffer, units));

        public void ClearSelection() => Send(SelectionRequestType.ClearSelection, null);

        public void ClearSelectionPreview() => Send(SelectionRequestType.ClearPreview, null);

        public void CommitSelection() => Send(SelectionRequestType.Commit, null);

        private static List<Entity> Fill(List<Entity> buffer, IReadOnlyList<Entity> units)
        {
            buffer.Clear();
            for (int i = 0; i < units.Count; i++)
                buffer.Add(units[i]);
            return buffer;
        }

        private void Send(SelectionRequestType type, List<Entity> units)
        {
            _requestStash.Set(_world.CreateEntity(), new SelectionRequest
            {
                Type = type,
                Units = units,
                Sequence = _sequence++
            });
        }
    }
}