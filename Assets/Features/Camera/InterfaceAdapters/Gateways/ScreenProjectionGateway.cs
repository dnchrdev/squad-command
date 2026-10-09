using Features.Camera.Infrastructure;
using Features.Shared.Projection;
using UnityEngine;

namespace Features.Camera.InterfaceAdapters.Gateways
{
    public sealed class ScreenProjectionGateway : IScreenProjection
    {
        private readonly ControlledCamera _camera;

        public ScreenProjectionGateway(ControlledCamera camera)
        {
            _camera = camera;
        }

        public Vector3 WorldToScreenPoint(Vector3 worldPoint) => _camera.Camera.WorldToScreenPoint(worldPoint);

        public Ray ScreenPointToRay(Vector2 screenPoint) => _camera.Camera.ScreenPointToRay(screenPoint);
    }
}