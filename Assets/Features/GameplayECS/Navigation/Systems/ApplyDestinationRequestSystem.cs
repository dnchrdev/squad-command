using Features.GameplayECS.Navigation.Components;
using Features.GameplayECS.Navigation.Requests;
using Features.GameplayECS.Navigation.Systems.Ports;
using Features.GameplayECS.Synchronization.Outputs;
using Scellecs.Morpeh;

namespace Features.GameplayECS.Navigation.Systems
{
    public sealed class ApplyDestinationRequestSystem : ISystem
    {
        private const int FIRST_PATH_CORNER_INDEX = 1;

        private readonly IPathService _pathService;

        public World World { get; set; }

        private Filter _destinationRequestFilter;

        private Stash<Destination> _destinationStash;
        private Stash<DestinationRequest> _destinationRequestStash;
        private Stash<Position> _positionStash;

        public ApplyDestinationRequestSystem(IPathService pathService)
        {
            _pathService = pathService;
        }

        public void OnAwake()
        {
            _destinationRequestFilter = World.Filter
                .With<DestinationRequest>()
                .Build();

            _destinationStash = World.GetStash<Destination>();
            _positionStash = World.GetStash<Position>();
            _destinationRequestStash = World.GetStash<DestinationRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _destinationRequestFilter)
            {
                ref var destinationRequest = ref _destinationRequestStash.Get(request);
                ref var position = ref _positionStash.Get(destinationRequest.Target);

                var destination = new Destination();

                if (_pathService.TryBuildPath(position.Value, destinationRequest.Destination, out var target, out var corners))
                {
                    destination.Corners = corners;
                    destination.CurrentCornerIndex = FIRST_PATH_CORNER_INDEX;
                    destination.HasPath = true;
                    destination.Target = target;
                }
                else
                {
                    destination.HasPath = false;
                    destination.Corners = null;
                }

                _destinationStash.Set(destinationRequest.Target, destination);
                _destinationRequestStash.Remove(request);
            }
        }

        public void Dispose()
        {
        }
    }
}