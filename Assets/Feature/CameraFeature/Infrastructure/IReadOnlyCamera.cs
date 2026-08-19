using UnityEngine;

namespace Feature.CameraFeature.Infrastructure
{
    public interface IReadOnlyCamera
    {
        Camera Camera { get; }
    }
}