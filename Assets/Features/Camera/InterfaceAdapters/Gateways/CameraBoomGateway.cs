using Features.Camera.Infrastructure;
using Features.Camera.UseCases.Ports;
using UnityEngine;

namespace Features.Camera.InterfaceAdapters.Gateways
{
    public sealed class CameraBoomGateway : ICameraBoom
    {
        private readonly CameraRefs _refs;

        public CameraBoomGateway(CameraRefs refs)
        {
            _refs = refs;
        }

        public void SetDistance(float distance)
        {
            _refs.BoomRoot.transform.localPosition = new Vector3(0f, 0f, -distance);
        }
    }
}