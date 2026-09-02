using Feature.GameplayECS.Common;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.AI;

namespace Feature.GameplayECS.Navigation.Systems
{
    public class ApplyDestinationRequest : ISystem
    {
        public World World { get; set; }

        private Filter _destinationRequestFilter;

        private Stash<Destination> _destinationStash;
        private Stash<DestinationRequest> _destinationRequestStash;
        private Stash<Position> _positionStash;

        private NavMeshPath _navMeshPath;
        
        private readonly float[] _sampleRadius = { 0.5f, 1f, 2f, 4f, 8f };

        public void OnAwake()
        {
            _destinationRequestFilter = World.Filter
                .With<DestinationRequest>()
                .Build();

            _destinationStash = World.GetStash<Destination>();
            _positionStash = World.GetStash<Position>();
            _destinationRequestStash = World.GetStash<DestinationRequest>();

            _navMeshPath = new NavMeshPath();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _destinationRequestFilter)
            {
                ref var destinationRequest = ref _destinationRequestStash.Get(request);
                ref var position = ref _positionStash.Get(destinationRequest.Target);

                Vector3 currentPosition = position.Value;
                Vector3 requestedPoint = destinationRequest.Destination;

                var destination = new Destination();

                if (TryResolveReachablePoint(currentPosition, requestedPoint, out Vector3 resolvedPoint))
                {
                    bool success = NavMesh.CalculatePath(
                        currentPosition,
                        resolvedPoint,
                        NavMesh.AllAreas,
                        _navMeshPath
                    );

                    if (success && _navMeshPath.status != NavMeshPathStatus.PathInvalid &&
                        _navMeshPath.corners.Length > 0)
                    {
                        destination.Corners = _navMeshPath.corners;
                        destination.CurrentCornerIndex = 1;
                        destination.HasPath = true;
                        destination.Target = resolvedPoint;
                    }
                    else
                    {
                        destination.HasPath = false;
                        destination.Corners = null;
                    }
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

        /// <summary>
        /// Пытается найти достижимую точку на NavMesh:
        /// 1) если запрошенная точка уже валидна и путь до неё полный — используем её;
        /// 2) если точка вне NavMesh — ищем ближайшую точку на NavMesh расширяющимся радиусом;
        /// 3) если путь до найденной точки только частичный (PathPartial) —
        ///    берём последнюю достижимую точку корнеров как цель (ближайшую доступную к желаемой).
        /// </summary>
        private bool TryResolveReachablePoint(Vector3 from, Vector3 requested, out Vector3 result)
        {
            result = requested;

            // Шаг 1: "прилипаем" requested-точку к NavMesh, если она не на нём
            Vector3 candidate = requested;
            bool onMesh = NavMesh.SamplePosition(requested, out NavMeshHit hit, 0.1f, NavMesh.AllAreas);

            if (!onMesh)
            {
                foreach (var radius in _sampleRadius)
                {
                    if (NavMesh.SamplePosition(requested, out hit, radius, NavMesh.AllAreas))
                    {
                        onMesh = true;
                        break;
                    }
                }
            }

            if (!onMesh)
                return false; // точка совсем далеко от любой навигационной поверхности

            candidate = hit.position;

            // Шаг 2: проверяем реальную достижимость через CalculatePath.
            // Если путь только частичный — берём последнюю точку пути (ближайшую доступную).
            if (NavMesh.CalculatePath(from, candidate, NavMesh.AllAreas, _navMeshPath))
            {
                if (_navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    result = candidate;
                    return true;
                }

                if (_navMeshPath.status == NavMeshPathStatus.PathPartial && _navMeshPath.corners.Length > 0)
                {
                    // ближайшая доступная точка на пути к недостижимой цели
                    result = _navMeshPath.corners[_navMeshPath.corners.Length - 1];
                    return true;
                }
            }

            return false;
        }

        public void Dispose()
        {
        }
    }
}