using UnityEngine;

namespace Features.Camera.UseCases.Ports
{
    public interface ICameraRotation
    {
        void SetRotation(Quaternion rotation);
    }
}