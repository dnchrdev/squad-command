using Feature.GameplayECS.Common;
using Feature.GameplayECS.Movement;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Navigation.Systems
{
    public class CalculateNavigationDirectionSystem : ISystem
    {
        public World World { get; set; }

        private Filter _filter;
        private Stash<Destination> _destinationStash;
        private Stash<NavigationDirection> _moveDirectionStash;
        private Stash<Position> _positionStash;

        private const float WaypointTolerance = 0.2f;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<Destination>()
                .With<Position>()
                .With<UnitMotorComponent>()
                .Build();

            _destinationStash = World.GetStash<Destination>();
            _moveDirectionStash = World.GetStash<NavigationDirection>();
            _positionStash = World.GetStash<Position>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _filter)
            {
                ref var destination = ref _destinationStash.Get(entity);
                ref var position = ref _positionStash.Get(entity);

                Vector3 currentPosition = position.Value;

                if (TryCalculateDirection(ref destination, currentPosition, out Vector3 direction))
                {
                    _moveDirectionStash.Set(entity, new NavigationDirection { Value = direction });
                }
                else if (_moveDirectionStash.Has(entity))
                {
                    _moveDirectionStash.Remove(entity);
                }
            }
        }

        private bool TryCalculateDirection(ref Destination destination, Vector3 currentPosition, out Vector3 direction)
        {
            direction = default;

            if (!destination.HasPath || destination.Corners == null || destination.Corners.Length == 0)
                return false;

            if (destination.CurrentCornerIndex >= destination.Corners.Length)
                return false;

            Vector3 toCorner = destination.Corners[destination.CurrentCornerIndex] - currentPosition;
            toCorner.y = 0f;

            if (toCorner.sqrMagnitude < WaypointTolerance * WaypointTolerance)
            {
                destination.CurrentCornerIndex++;

                if (destination.CurrentCornerIndex >= destination.Corners.Length)
                    return false;

                toCorner = destination.Corners[destination.CurrentCornerIndex] - currentPosition;
                toCorner.y = 0f;
            }

            if (toCorner.sqrMagnitude <= 0.0001f)
                return false;

            direction = toCorner.normalized;
            return true;
        }

        public void Dispose()
        {
        }
    }
}