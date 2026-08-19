using UnityEngine;

namespace Feature.GameLifecycle.Application.Interfaces
{
    public interface ICameraRotationReset
    {
        void SetRotation(Quaternion targetRotation);
    }
}