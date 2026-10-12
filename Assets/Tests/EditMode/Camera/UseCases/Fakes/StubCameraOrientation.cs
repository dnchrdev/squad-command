using Features.Camera.UseCases.Ports;
using UnityEngine;

namespace Tests.EditMode.Camera.UseCases.Fakes
{
    public sealed class StubCameraOrientation : IReadOnlyCameraOrientation
    {
        public Vector3 Forward { get; set; } = Vector3.forward;
        public Vector3 Right { get; set; } = Vector3.right;
    }
}
