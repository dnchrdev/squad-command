using Features.Camera.Infrastructure;
using Features.Camera.UseCases.Ports;
using UnityEngine;

namespace Features.Camera.InterfaceAdapters.Gateways
{
    public sealed class CameraPositionGateway : ICameraPosition
    {
        private readonly CameraRefs _refs;

        public CameraPositionGateway(CameraRefs refs)
        {
            _refs = refs;
        }

        public void SetPosition(Vector3 position)
        {
            _refs.PositionRoot.transform.position = position;
        }
    }
}