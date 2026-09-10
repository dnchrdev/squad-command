using Feature.GameplayECS.Common;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.SelectCommand.Systems
{
    public class CommitSelectedSystem : ISystem
    {
        public World World { get; set; }
        private Filter _commitRequestFilter;
        private Filter _previewSelectedFilter;
        private Filter _previewUnselectedFilter;
        private Stash<CommitSelectedRequest> _commitRequestStash;
        private Stash<Selected> _selectedStash;
        private Stash<SelectSelfRequest> _selectSelfStash;
        private Stash<UnselectSelfRequest> _unselectSelfStash;
        private Stash<SelectedViewShowed> _selectedShowedStash;
        private Stash<SelectViewComponent> _selectViewStash;

        public void OnAwake()
        {
            _commitRequestFilter = World.Filter.With<CommitSelectedRequest>().Build();
            _previewSelectedFilter = World.Filter
                .With<UnitTag>().With<SelectSelfRequest>().With<SelectViewComponent>().Without<Selected>().Build();
            _previewUnselectedFilter = World.Filter
                .With<UnitTag>().With<UnselectSelfRequest>().With<SelectViewComponent>().Build();
            _commitRequestStash = World.GetStash<CommitSelectedRequest>();
            _selectedStash = World.GetStash<Selected>();
            _selectSelfStash = World.GetStash<SelectSelfRequest>();
            _unselectSelfStash = World.GetStash<UnselectSelfRequest>();
            _selectedShowedStash = World.GetStash<SelectedViewShowed>();
            _selectViewStash = World.GetStash<SelectViewComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            bool hasCommit = false;
            foreach (var _ in _commitRequestFilter) hasCommit = true;
            if (!hasCommit) return;

            foreach (var entity in _previewSelectedFilter)
            {
                ShowSelectedView(entity);
                _selectSelfStash.Remove(entity);
                _selectedStash.Set(entity);
            }

            foreach (var entity in _previewUnselectedFilter)
            {
                HideSelectedView(entity);
                _selectedStash.Remove(entity);
                _unselectSelfStash.Remove(entity);
            }

            RequestUtils.ConsumeAllRequests(World, _commitRequestFilter, _commitRequestStash);
        }

        private void ShowSelectedView(Entity entity)
        {
            if (_selectedShowedStash.Has(entity)) return;
            _selectViewStash.Get(entity).Value.Show();
            _selectedShowedStash.Add(entity);
        }

        private void HideSelectedView(Entity entity)
        {
            if (!_selectedShowedStash.Has(entity)) return;
            _selectViewStash.Get(entity).Value.Hide();
            _selectedShowedStash.Remove(entity);
        }

        public void Dispose()
        {
        }
    }
}