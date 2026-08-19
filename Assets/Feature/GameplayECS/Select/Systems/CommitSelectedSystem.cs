using Feature.GameplayECS.Common;
using Feature.GameplayECS.View;
using Scellecs.Morpeh;
using Position = UnityEngine.UIElements.Position;

namespace Feature.GameplayECS.Select.Systems
{
    public class CommitSelectedSystem : ISystem
    {
        public World World { get; set; }
        private Filter _selectCommitRequest;
        private Filter _selectedSelfFilter;

        public Stash<Selected> _selectedStash;
        public Stash<SelectSelfRequest> _selectSelfRequestStash;
        //private Stash<SelectedViewShowed> _selectedShowedStash;
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
                //.Without<SelectedViewShowed>()
                .Without<Selected>()
                .Build();

            _selectedStash = World.GetStash<Selected>();
            _commitSelectedRequest = World.GetStash<CommitSelectedRequest>();
            _selectSelfRequestStash = World.GetStash<SelectSelfRequest>();
            //_selectedShowedStash = World.GetStash<SelectedViewShowed>();
            _selectViewStash = World.GetStash<SelectViewComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _selectCommitRequest)
            {
                foreach (var entity in _selectedSelfFilter)
                {
                    // ref var selectedView = ref _selectViewStash.Get(entity);
                    // selectedView.Value.Show();
                    _selectSelfRequestStash.Remove(entity);
                    _selectedStash.Add(entity);
                }
                
                _commitSelectedRequest.Remove(request);
            }
        }

        public void Dispose()
        {
        }
    }
}