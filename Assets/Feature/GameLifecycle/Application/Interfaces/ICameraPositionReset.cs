using UnityEngine;

namespace Feature.GameLifecycle.Application.Interfaces
{
    public interface ICameraPositionReset
    {
        void SetPosition(Vector3 targetPosition);
    }
}