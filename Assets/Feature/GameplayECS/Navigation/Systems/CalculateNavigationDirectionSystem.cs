using Feature.GameplayECS.Common;
using Feature.GameplayECS.Movement;
using Scellecs.Morpeh;
using UnityEngine;

namespace Feature.GameplayECS.Navigation.Systems
{
    /// <summary>
    /// Считает желаемое направление движения по текущему сегменту пути (Destination.Corners)
    /// и сразу пишет его в SteeringDirection. LocalAvoidanceSystem убрана из пайплайна —
    /// коллизии/расталкивание юнитов решаются отдельной системой, поэтому
    /// NavigationDirection как промежуточный компонент больше не нужен.
    /// </summary>
    public class CalculateNavigationDirectionSystem : ISystem
    {
        private const float FINAL_WAYPOINT_TOLERANCE = 0.1f;
        private const float WAYPOINT_TOLERANCE = 1f;
        
        public World World { get; set; }

        private Filter _filter;
        private Stash<Destination> _destinationStash;
        private Stash<Position> _positionStash;
        private Stash<SteeringDirection> _steeringStash;

        public void OnAwake()
        {
            _filter = World.Filter
                .With<Destination>()
                .With<Position>()
                .With<UnitMotorComponent>()
                .Build();

            _destinationStash = World.GetStash<Destination>();
            _positionStash = World.GetStash<Position>();
            _steeringStash = World.GetStash<SteeringDirection>();
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
                    _steeringStash.Set(entity, new SteeringDirection { Value = direction });
                }
                else
                {
                    // цель достигнута (или путь невозможен) — явно гасим движение,
                    // а не оставляем "зависший" вектор в SteeringDirection
                    if (_steeringStash.Has(entity))
                        _steeringStash.Set(entity, new SteeringDirection { Value = Vector3.zero });

                    // путь исчерпан — сама Destination больше не нужна
                    if (_destinationStash.Has(entity))
                        _destinationStash.Remove(entity);
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

            float tolerance = destination.CurrentCornerIndex == destination.Corners.Length - 1 ? FINAL_WAYPOINT_TOLERANCE : WAYPOINT_TOLERANCE;
            
            if (toCorner.sqrMagnitude < tolerance * tolerance)
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