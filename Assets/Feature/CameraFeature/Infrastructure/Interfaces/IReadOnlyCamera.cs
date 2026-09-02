using UnityEngine;

namespace Feature.CameraFeature.Infrastructure.Interfaces
{
    public interface IReadOnlyCamera
    {
        Camera Camera { get; }
    }
}