using Features.Camera.Infrastructure;
using Features.Camera.UseCases.Ports;
using UnityEngine;

namespace Features.Camera.InterfaceAdapters.Gateways
{
    public sealed class CameraRotationGateway : ICameraRotation, IReadOnlyCameraOrientation
    {
        private readonly CameraRefs _refs;

        public CameraRotationGateway(CameraRefs refs)
        {
            _refs = refs;
        }

        public Vector3 Forward => _refs.RotationRoot.forward;
        public Vector3 Right => _refs.RotationRoot.right;

        public void SetRotation(Quaternion rotation)
        {
            _refs.RotationRoot.transform.rotation = rotation;
        }
    }
}