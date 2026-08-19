using Feature.CameraFeature.Infrastructure;
using Feature.Command.Adapter.Interfaces;
using Feature.Command.Domain;
using Feature.Command.Infrastructure.Configs;
using UnityEngine;
using Zenject;

namespace Feature.Command.Infrastructure
{
    public sealed class GroundQueryService : IGroundQueryService
    {
        [Inject] private readonly IReadOnlyCamera _camera;
        [Inject] private readonly CommandConfig _config;

        public bool TryQueryGroundPoint(Vector2 screenPoint, out Vector3 worldPoint)
        {
            var ray = _camera.Camera.ScreenPointToRay(screenPoint);
            if (Physics.Raycast(ray, out var hit, 500f, _config.GroundMask))
            {
                worldPoint = hit.point;
                return true;
            }

            worldPoint = default;
            return false;
        }
    }
}