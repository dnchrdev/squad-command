using Feature.GameplayECS.Common;
using Feature.GameplayECS.Movement;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.AI;

namespace Feature.GameplayECS.Navigation.Systems
{
    public class RecalculatePathSystem: ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<Destination> _destinationStash;
        private Stash<RecalculateSelfPathRequest> _requestStash;
        private Stash<Position> _positionStash;
        
        private NavMeshPath _navMeshPath;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<Destination>()
                .With<UnitMotorComponent>()
                .With<Position>()
                .With<RecalculateSelfPathRequest>()
                .Build();

            _destinationStash = World.GetStash<Destination>();
            _positionStash = World.GetStash<Position>();
            _requestStash = World.GetStash<RecalculateSelfPathRequest>();

            _navMeshPath = new NavMeshPath();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var destination = ref _destinationStash.Get(entity);
                ref var position = ref _positionStash.Get(entity);

                Vector3 currentPosition = position.Value;

                bool success = NavMesh.CalculatePath(
                    currentPosition,
                    destination.Target,
                    NavMesh.AllAreas,
                    _navMeshPath
                );

                if (success && _navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    destination.Corners = _navMeshPath.corners;
                    destination.CurrentCornerIndex = 1;
                    destination.HasPath = true;
                }
                else
                {
                    destination.HasPath = false;
                    destination.Corners = null;
                }

                _requestStash.Remove(entity);
            }
        }

        public void Dispose() { }
    }
}