using UnityEngine;

namespace Feature.CameraFeature.Adapter.Interfaces
{
    public interface ICameraRotationUpdate
    {
        void Tick(float dt, Vector2 delta);
    }
}