using UnityEngine;

namespace Feature.CameraFeature.Application.Interfaces
{
    public interface IReadOnlyCameraOrientation
    {
        Vector3 Forward { get; }
        Vector3 Right { get; }
    }
}