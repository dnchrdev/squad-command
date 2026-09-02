using Feature.GameplayECS.Common;
using Scellecs.Morpeh;

namespace Feature.GameplayECS.SelectCommand.Systems
{
    public class CommitSelectedSystem : ISystem
    {
        public World World { get; set; }
        private Filter _selectCommitRequest;
        private Filter _selectedSelfFilter;
        private Filter _unselectedSelfFilter;

        public Stash<Selected> _selectedStash;
        public Stash<SelectSelfRequest> _selectSelfRequestStash;
        public Stash<UnselectSelfRequest> _unselectSelfRequestStash;
        private Stash<SelectedViewShowed> _selectedShowedStash;
        private Stash<SelectViewComponent> _selectViewStash;
        private Stash<CommitSelectedRequest> _commitSelectedRequest;

        public void OnAwake()
        {
            _selectCommitRequest = World.Filter
                .With<CommitSelectedRequest>()
                .Build();

            _selectedSelfFilter = World.Filter
                .With<UnitTag>()
                .With<SelectSelfRequest>()
                .With<SelectViewComponent>()
                .Without<Selected>()
                .Build();
            
            _unselectedSelfFilter = World.Filter
                .With<UnitTag>()
                .With<UnselectSelfRequest>()
                .With<SelectViewComponent>()

                .Build();

            _selectedStash = World.GetStash<Selected>();
            _commitSelectedRequest = World.GetStash<CommitSelectedRequest>();
            _selectSelfRequestStash = World.GetStash<SelectSelfRequest>();
            _unselectSelfRequestStash = World.GetStash<UnselectSelfRequest>();
            _selectedShowedStash = World.GetStash<SelectedViewShowed>();
            _selectViewStash = World.GetStash<SelectViewComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _selectCommitRequest)
            {
                foreach (var entity in _selectedSelfFilter)
                {
                    ref var selectedView = ref _selectViewStash.Get(entity);

                    if (_selectedShowedStash.Has(entity) == false)
                    {
                        selectedView.Value.Show();
                        _selectedShowedStash.Add(entity);
                    }
                    
                    _selectSelfRequestStash.Remove(entity);
                    _selectedStash.Set(entity);
                }
                
                foreach (var entity in _unselectedSelfFilter)
                {
                    ref var selectedView = ref  _selectViewStash.Get(entity);
                    
                    
                    if(_selectedShowedStash.Has(entity))
                    {
                        selectedView.Value.Hide();
                        _selectedShowedStash.Remove(entity);
                    }

                    if (_selectedStash.Has(entity))
                    {
                        _selectedStash.Remove(entity);
                    }
                    
                    _unselectSelfRequestStash.Remove(entity);
                }

                
                _commitSelectedRequest.Remove(request);
            }
        }

        public void Dispose()
        {
        }
    }
}