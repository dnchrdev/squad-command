using UnityEngine;

namespace Feature.CameraFeature.Adapter.Interfaces
{
    public interface ICameraPositionUpdate
    {
        void Tick(float dt, Vector2 delta);
    }
}