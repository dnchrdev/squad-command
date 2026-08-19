using UnityEngine;

namespace Feature.GameplayECS.Movement
{
    public interface IUnitMotor
    {
        Vector3 Position { get; }
        Quaternion Rotation { get; }
        Vector3 Velocity { get; }
        
        void SetPosition(Vector3 position);
        void SetVelocity(Vector3 velocity);
    }
}