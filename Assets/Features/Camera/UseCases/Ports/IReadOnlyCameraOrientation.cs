using UnityEngine;

namespace Features.Camera.UseCases.Ports
{
    public interface IReadOnlyCameraOrientation
    {
        Vector3 Forward { get; }
        Vector3 Right { get; }
    }
}