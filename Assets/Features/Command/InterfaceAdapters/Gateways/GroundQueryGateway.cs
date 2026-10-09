using Features.Command.UseCases.Ports;
using Features.Shared.Projection;
using UnityEngine;

namespace Features.Command.InterfaceAdapters.Gateways
{
    public sealed class GroundQueryGateway : IGroundQuery
    {
        private readonly IScreenProjection _projection;
        private readonly LayerMask _groundMask;
        private readonly float _maxDistance;

        public GroundQueryGateway(IScreenProjection projection, LayerMask groundMask, float maxDistance)
        {
            _projection = projection;
            _groundMask = groundMask;
            _maxDistance = maxDistance;
        }

        public bool TryGetGroundPoint(Vector2 screenPoint, out Vector3 worldPoint)
        {
            var ray = _projection.ScreenPointToRay(screenPoint);
            if (UnityEngine.Physics.Raycast(ray, out var hit, _maxDistance, _groundMask))
            {
                worldPoint = hit.point;
                return true;
            }

            worldPoint = default;
            return false;
        }
    }
}