using Features.GameplayECS.Navigation.Systems.Ports;
using UnityEngine;
using UnityEngine.AI;

namespace Features.GameplayECS.Navigation.InterfaceAdapters.Gateways
{
    public sealed class NavMeshPathGateway : IPathService
    {
        private const float ON_MESH_SAMPLE_RADIUS = 0.1f;

        private readonly NavMeshPath _navMeshPath = new NavMeshPath();
        private readonly float[] _sampleRadius = { 0.5f, 1f, 2f, 4f, 8f };

        public bool TryBuildPath(Vector3 from, Vector3 requested, out Vector3 target, out Vector3[] corners)
        {
            target = default;
            corners = null;

            if (!TryResolveReachablePoint(from, requested, out Vector3 resolvedPoint))
                return false;

            bool success = NavMesh.CalculatePath(from, resolvedPoint, NavMesh.AllAreas, _navMeshPath);

            if (!success || _navMeshPath.status == NavMeshPathStatus.PathInvalid || _navMeshPath.corners.Length == 0)
                return false;

            corners = _navMeshPath.corners;
            target = resolvedPoint;
            return true;
        }

        private bool TryResolveReachablePoint(Vector3 from, Vector3 requested, out Vector3 result)
        {
            result = requested;

            bool onMesh = NavMesh.SamplePosition(requested, out NavMeshHit hit, ON_MESH_SAMPLE_RADIUS, NavMesh.AllAreas);

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
                return false;

            Vector3 candidate = hit.position;

            if (NavMesh.CalculatePath(from, candidate, NavMesh.AllAreas, _navMeshPath))
            {
                if (_navMeshPath.status == NavMeshPathStatus.PathComplete)
                {
                    result = candidate;
                    return true;
                }

                if (_navMeshPath.status == NavMeshPathStatus.PathPartial && _navMeshPath.corners.Length > 0)
                {
                    result = _navMeshPath.corners[_navMeshPath.corners.Length - 1];
                    return true;
                }
            }

            return false;
        }
    }
}